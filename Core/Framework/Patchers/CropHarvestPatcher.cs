namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class CropHarvestPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CropHarvestPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CropHarvestPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Crop>(nameof(Crop.harvest));
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CropHarvestPostifx(
        Crop __instance, ref bool __result, int xTile, int yTile, HoeDirt soil, JunimoHarvester junimoHarvester, bool isForcedScytheHarvest)
    {
        if (__instance.indexOfHarvest.Value == "262" && __instance.currentPhase.Value == __instance.phaseDays.Count - 2 &&
            junimoHarvester is null && isForcedScytheHarvest && Config.ImmersiveHay)
        {
            soil?.Location?.playSound("cut");
            var hayItem = ItemRegistry.Create(QIDs.Hay);
            Game1.createItemDebris(hayItem.getOne(), new Vector2((xTile * Game1.tileSize) + 32, (yTile * Game1.tileSize) + 32), -1);
            if (Game1.player.professions.Contains(Farmer.tiller) && ModHelper.ModRegistry.IsLoaded("DaLion.Professions"))
            {
                var chance = Game1.player.professions.Contains(Farmer.tiller + 100) ? 0.2 : 0.1;
                if (Game1.random.NextBool(chance))
                {
                    Game1.createItemDebris(hayItem.getOne(), new Vector2((xTile * Game1.tileSize) + 32, (yTile * Game1.tileSize) + 32), -1);
                }
            }

            __result = true;
        }
    }

    /// <summary>Patch to remove regular hay harvest from wheat.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? CropHarvestTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var unimmersive = generator.DefineLabel();
            var immersive = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[10]),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Random).RequireMethod(nameof(Random.NextDouble))),
                    new CodeInstruction(OpCodes.Ldc_R8, 0.4),
                    new CodeInstruction(OpCodes.Bge_Un)
                ])
                .Move(2)
                .AddLabels(unimmersive)
                .Insert([
                    new CodeInstruction(OpCodes.Call, typeof(CoreMod).RequirePropertyGetter(nameof(Config))),
                    new CodeInstruction(OpCodes.Call, typeof(CoreConfig).RequirePropertyGetter(nameof(Config.ImmersiveHay))),
                    new CodeInstruction(OpCodes.Brfalse_S, unimmersive),
                    new CodeInstruction(OpCodes.Ldc_R8, 0d),
                    new CodeInstruction(OpCodes.Br_S, immersive)
                ])
                .Move()
                .AddLabels(immersive);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing base hay harvest from Wheat.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var notWinterWheat = generator.DefineLabel();
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldstr, "421")
                    ],
                    ILHelper.SearchOption.Last)
                .Move(2)
                .GetOperand(out var not421)
                .LabelMatch((Label)not421)
                .StripLabels()
                .AddLabels(notWinterWheat)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(CropHarvestPatcher).RequireMethod(nameof(IsWinterWheat))),
                        new CodeInstruction(OpCodes.Brfalse_S, notWinterWheat),
                        new CodeInstruction(OpCodes.Ldloca_S, helper.Locals[15]),
                        new CodeInstruction(OpCodes.Ldc_I4_2),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Stloc_S, helper.Locals[15]),
                    ],
                    [(Label)not421]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Winter Wheat harvest.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static bool IsWinterWheat(Crop crop)
    {
        return crop.indexOfHarvest.Value == QIDs.Wheat && Data.ReadAs<bool>(crop, DataKeys.WinterWheat);
    }

    #endregion injected
}
