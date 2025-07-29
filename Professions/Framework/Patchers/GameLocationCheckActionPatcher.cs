namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationCheckActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCheckActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationCheckActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.checkAction));
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static void GameLocationCheckActionPrefix(GameLocation __instance, xTile.Dimensions.Location tileLocation, Farmer who)
    {
        if (!who.HasProfession(Profession.Piper))
        {
            return;
        }

        var tileRect = new Rectangle(
            tileLocation.X * Game1.tileSize,
            tileLocation.Y * Game1.tileSize,
            Game1.tileSize,
            Game1.tileSize);
        if ((__instance is Farm or SlimeHutch || __instance.IsOutdoors) && who.CountPipedSlimes() == 0)
        {
            foreach (var slime in __instance.characters.OfType<GreenSlime>())
            {
                if (slime.GetBoundingBox().Intersects(tileRect))
                {
                    slime.CheckActionNonPiped(who);
                }
            }
        }
        else
        {
            foreach (var piped in who.GetPipedSlimes())
            {
                if (piped.Slime.GetBoundingBox().Intersects(tileRect))
                {
                    piped.CheckAction(who);
                }
            }
        }
    }

    /// <summary>Patch for Gemologist to harvest quality minerals + prestiged Forager double forage.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GameLocationCheckActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Removed: if (obj.isForage())
        try
        {
            var doGetHarvestSpawnedObjectQuality = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Callvirt, typeof(SObject).RequireMethod(nameof(SObject.isForage))),
                    new CodeInstruction(OpCodes.Brfalse_S),
                ])
                .Move(2)
                .GetOperand(out var notForageNorForagedMineral)
                .ReplaceWith(new CodeInstruction(OpCodes.Brtrue_S, doGetHarvestSpawnedObjectQuality))
                .Move()
                .AddLabels(doGetHarvestSpawnedObjectQuality)
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ItemExtensions).RequireMethod(nameof(ItemExtensions.IsForagedMineral))),
                    new CodeInstruction(OpCodes.Brfalse_S, notForageNorForagedMineral),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding quality to foraged minerals.\nHelper returned {ex}");
            return null;
        }

        // From: if (random.NextDouble() < 0.2)
        // To: if (random.NextDouble() < who.professions.Contains(100 + <forager_id>) ? 0.4 : 0.2
        try
        {
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Farmer.gatherer)
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_R8, 0.2)])
                .AddLabels(isNotPrestiged)
                .Insert([new CodeInstruction(OpCodes.Ldarg_3)])
                .InsertProfessionCheck(Farmer.gatherer + 100, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_R8, 0.4),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                ])
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Forager double forage bonus.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
