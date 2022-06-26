namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeShakePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal TreeShakePatch()
    {
        Target = RequireMethod<Tree>("shake");
    }

    #region harmony patches

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? TreeShakeTranspiler(IEnumerable<CodeInstruction> instructions,
            MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: Game1.createObjectDebris(seedIndex, tileLocation.X, tileLocation.Y - 3, (tileLocation.Y + 1) * 64, 0, 1f, location);
        /// To: Game1.createObjectDebris(seedIndex, tileLocation.X, tileLocation.Y - 3, (tileLocation.Y + 1) * 64, GetCoconutQuality(), 1f, location);
        ///     -- and again for golden coconut immediately below

        try
        {
            var callCreateObjectDebrisInst = new CodeInstruction(OpCodes.Call,
                typeof(Game1).RequireMethod(nameof(Game1.createObjectDebris),
                    new[]
                    {
                            typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float),
                            typeof(GameLocation)
                    }));

            helper
                // the normal coconut
                .FindFirst(callCreateObjectDebrisInst)
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_0)
                )
                .ReplaceWith(
                    new CodeInstruction(OpCodes.Call,
                        typeof(TreeShakePatch).RequireMethod(nameof(GetCoconutQuality)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_2)
                )
                // the golden coconut
                .FindNext(
                    new CodeInstruction(OpCodes.Ldc_I4, 791)
                )
                .AdvanceUntil(callCreateObjectDebrisInst)
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_0)
                )
                .ReplaceWith(
                    new CodeInstruction(OpCodes.Call,
                        typeof(TreeShakePatch).RequireMethod(nameof(GetCoconutQuality)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_I4, 791)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed applying Ecologist/Botanist perk to shaken coconut.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int GetCoconutQuality(int seedIndex)
    {
        if (seedIndex is not (Constants.COCONUT_INDEX_I or Constants.GOLDEN_COCONUT_INDEX_I) ||
            !ModEntry.Config.ProfessionalForagingInGingerIsland ||
            !Game1.player.professions.Contains(Farmer.botanist))
            return SObject.lowQuality;

        return ModEntry.ProfessionsAPI is null
            ? SObject.bestQuality
            : ModEntry.ProfessionsAPI.GetEcologistForageQuality(Game1.player);
    }

    #endregion injected subroutines
}