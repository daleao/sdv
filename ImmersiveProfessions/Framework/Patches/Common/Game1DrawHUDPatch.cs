namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class Game1DrawHUDPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal Game1DrawHUDPatch()
    {
        Original = RequireMethod<Game1>("drawHUD");
    }

    #region harmony patches

    /// <summary>Patch for Prospector to track ladders and shafts + Scavenger to track berry bushes.</summary>
    [HarmonyPostfix]
    private static void Game1DrawHUDPostfix()
    {
        // track ladders and shafts as Prospector
        if (Game1.player.HasProfession(Profession.Prospector) && Game1.currentLocation is MineShaft shaft)
            foreach (var tile in shaft.GetLadderTiles())
                ModEntry.PlayerState.Pointer.DrawAsTrackingPointer(tile, Color.Lime);
        
        // track berry bushes as Scavenger
        else if (Game1.player.HasProfession(Profession.Scavenger) && Game1.currentLocation is {IsOutdoors: true} outdoors)
            foreach (var bush in outdoors.largeTerrainFeatures.OfType<Bush>().Where(b =>
                         !b.townBush.Value && b.tileSheetOffset.Value == 1 &&
                         b.inBloom(Game1.GetSeasonForLocation(outdoors), Game1.dayOfMonth)))
                ModEntry.PlayerState.Pointer.DrawAsTrackingPointer(bush.tilePosition.Value, Color.Yellow);
    }

    /// <summary>Patch for Scavenger and Prospector to track different stuff.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Game1DrawHUDTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (!player.professions.Contains(<scavenger_id>) || !currentLocation.IsOutdoors) return
        /// To: if (!(player.professions.Contains(<scavenger_id>) || player.professions.Contains(<prospector_id>)) return

        var isProspector = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck(Farmer.tracker) // find index of tracker check
                .Retreat()
                .GetInstructionsUntil(
                    out var got,
                    pattern: new CodeInstruction(OpCodes.Brfalse) // copy profession check
                )
                .Insert(got) // paste
                .Return()
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_S)
                )
                .SetOperand((int) Profession.Prospector) // change to prospector check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse)
                )
                .ReplaceWith(
                    new(OpCodes.Brtrue_S, isProspector) // change !(A && B) to !(A || B)
                )
                .Advance()
                .RemoveLabels() // remove repeated label
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Call,
                        typeof(Game1).RequirePropertyGetter(nameof(Game1.currentLocation)))
                )
                .Remove(3) // remove currentLocation.IsOutdoors check
                .AddLabels(isProspector); // branch here is first profession check was true
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded tracking pointers draw condition. Helper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// From: if ((bool)pair.Value.isSpawnedObject || pair.Value.ParentSheetIndex == 590) ...
        /// To: if (_ShouldDraw(pair.Value)) ...

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Bne_Un) // find branch to loop head
                )
                .GetOperand(out var loopHead) // copy destination
                .RetreatUntil(
#pragma warning disable AvoidNetField // Avoid Netcode types when possible
                    new CodeInstruction(OpCodes.Ldfld,
                        typeof(SObject).RequireField(nameof(SObject.isSpawnedObject)))
#pragma warning restore AvoidNetField // Avoid Netcode types when possible
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes
                        .Bne_Un) // remove pair.Value.isSpawnedObject || pair.Value.ParentSheetIndex == 590
                )
                .Insert( // insert call to custom condition
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.ShouldBeTracked))),
                    new CodeInstruction(OpCodes.Brfalse, loopHead)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded tracking pointers draw condition. Helper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// Injected: if (!player.professions.Contains(<prospector_id>)) return
        /// Before panning tracker

        var drawPanningTracker = generator.DefineLabel();
        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.currentLocation))),
                    new CodeInstruction(OpCodes.Ldfld, typeof(GameLocation).RequireField(nameof(GameLocation.orePanPoint))),
                    new CodeInstruction(OpCodes.Call, typeof(Point).RequirePropertyGetter(nameof(Point.Zero))),
                    new CodeInstruction(OpCodes.Box)
                )
                .StripLabels(out var labels)
                .AddLabels(drawPanningTracker)
                .InsertProfessionCheck((int) Profession.Prospector, labels)
                .Insert(
                    new CodeInstruction(OpCodes.Brtrue_S, drawPanningTracker),
                    new CodeInstruction(OpCodes.Ret)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching Prospector restriction for panning tacker. Helper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}