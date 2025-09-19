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
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationCheckActionPatcher : HarmonyPatcher
{
    private static GreenSlime? _paintBrushTarget;

    /// <summary>Initializes a new instance of the <see cref="GameLocationCheckActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationCheckActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.checkAction));
    }

    #region harmony patches

    /// <summary>Patch to perform custom Piper interactions with Slimes.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool GameLocationCheckActionPrefix(GameLocation __instance, ref bool __result, xTile.Dimensions.Location tileLocation, Farmer who)
    {
        if (!who.HasProfession(Profession.Piper))
        {
            __result = false;
            return true; // run original logic
        }

        var tileRect = new Rectangle(
            tileLocation.X * Game1.tileSize,
            tileLocation.Y * Game1.tileSize,
            Game1.tileSize,
            Game1.tileSize);

        if (__instance is Farm or SlimeHutch && who.HasProfession(Profession.Piper, true) &&
            who.Items.Count > who.CurrentToolIndex && who.Items[who.CurrentToolIndex] is SObject @object && @object.HasContextTag("slime_painter_item"))
        {
            foreach (var slime in __instance.characters.OfType<GreenSlime>())
            {
                if (!slime.GetBoundingBox().Intersects(tileRect))
                {
                    continue;
                }

                _paintBrushTarget = slime;
                __instance.createQuestionDialogue(I18n.Piper_Usebrush(), __instance.createYesNoResponses(), HandlePaintBursh);
                __result = true;
                return false; // don't run original logic
            }
        }

        if (!who.HasProfession(Profession.Piper) || who.Items.Count <= who.CurrentToolIndex ||
            who.Items[who.CurrentToolIndex] is not Hat hat)
        {
            __result = false;
            return true; // run original logic
        }

        if ((__instance is Farm or SlimeHutch || __instance.IsOutdoors) && who.CountPipedSlimes() == 0)
        {
            foreach (var slime in __instance.characters.OfType<GreenSlime>())
            {
                if (!slime.GetBoundingBox().Intersects(tileRect))
                {
                    continue;
                }

                slime.GiveHatNonPiped(who, hat);
                __result = true;
                return false; // don't run original logic
            }

            __result = false;
            return true; // run original logic
        }

        foreach (var piped in who.GetPipedSlimes())
        {
            if (piped.Slime.GetBoundingBox().Intersects(tileRect))
            {
                piped.CheckAction(who, hat);
                __result = true;
                return false; // don't run original logic
            }
        }

        __result = false;
        return true; // run original logic
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

    private static void HandlePaintBursh(Farmer who, string choice)
    {
        if (choice == "No" || _paintBrushTarget is null)
        {
            return;
        }

        var paintBrush = (who.Items[who.CurrentToolIndex] as SObject)!;
        var color = Color.White;
        if (paintBrush.ItemId == RedBrushId)
        {
            color = new Color(0.627f, 0.075f, 0.075f);
        }
        else if (paintBrush.ItemId == GreenBrushId)
        {
            color = new Color(0f, 0.706f, 0f);
        }
        else if (paintBrush.ItemId == BlueBrushId)
        {
            color = new Color(0.22f, 0.561f, 0.765f);
        }
        else if (paintBrush.ItemId == PurpleBrushId)
        {
            color = new Color(0.494f, 0.196f, 0.502f);
        }

        if (_paintBrushTarget.TryUsePaintbrush(paintBrush))
        {
            var multiplayer = Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke();
            multiplayer.broadcastSprites(
                _paintBrushTarget.currentLocation,
                new TemporaryAnimatedSprite(44, _paintBrushTarget.Position, color * 0.66f, 10)
                {
                    interval = 70f, holdLastFrame = true,
                    alphaFade = 0.01f,
                });
            multiplayer.broadcastSprites(
                _paintBrushTarget.currentLocation,
                new TemporaryAnimatedSprite(44, _paintBrushTarget.Position + new Vector2(-16f, 0f), color * 0.66f, 10)
                {
                    interval = 70f, delayBeforeAnimationStart = 0, holdLastFrame = true,
                    alphaFade = 0.01f,
                });
            multiplayer.broadcastSprites(
                _paintBrushTarget.currentLocation,
                new TemporaryAnimatedSprite(44, _paintBrushTarget.Position + new Vector2(0f, 16f), color * 0.66f, 10)
                {
                    interval = 70f, delayBeforeAnimationStart = 100, holdLastFrame = true,
                    alphaFade = 0.01f,
                });
            multiplayer.broadcastSprites(
                _paintBrushTarget.currentLocation,
                new TemporaryAnimatedSprite(44, _paintBrushTarget.Position + new Vector2(16f, 0f), color * 0.66f, 10)
                {
                    interval = 70f, delayBeforeAnimationStart = 200, holdLastFrame = true,
                    alphaFade = 0.01f,
                });
            Game1.playSound("slimeHit");
            who.Items[who.CurrentToolIndex] = paintBrush.ConsumeStack(1);
        }
        else
        {
            Game1.playSound("cancel");
            Game1.addHUDMessage(new HUDMessage(I18n.Piper_Usebrush_Cant(), HUDMessage.error_type));
        }

        _paintBrushTarget = null;
    }
}
