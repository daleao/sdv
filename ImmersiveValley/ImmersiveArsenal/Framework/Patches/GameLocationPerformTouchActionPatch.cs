namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformTouchActionPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationPerformTouchActionPatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.performTouchAction));
    }

    #region harmony patches

    /// <summary>Apply new galaxy sword conditions.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationPerformTouchActionTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, 74)
        /// To: if (DoesPlayerMeetGalaxyConditions())
        ///     -- and also
        /// Injected: this.playSound("thunder");

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.ActiveObject)))
                )
                .StripLabels(out var labels)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse)
                )
                .GetOperand(out var didNotMeetConditions)
                .Return()
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Brtrue)
                )
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocationPerformTouchActionPatch).RequireMethod(
                            nameof(DoesPlayerMeetGalaxyConditions))),
                    new CodeInstruction(OpCodes.Brfalse, didNotMeetConditions),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldstr, "thunder"),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocation).RequireMethod(nameof(GameLocation.playSound)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting custom galaxy sword conditions.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool DoesPlayerMeetGalaxyConditions()
    {
        if (Game1.player.ActiveObject is null ||
            !StardewValley.Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, Constants.PRISMATIC_SHARD_INDEX_I) ||
            Game1.player.mailReceived.Contains("galaxySword"))
            return false;

        if (ModEntry.Config.InfinityPlusOneWeapons)
        {
            return Game1.player.Items.Any(item =>
                       item?.ParentSheetIndex == Constants.IRIDIUM_BAR_INDEX_I && item.Stack >= 10) &&
                   StardewValley.Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject,
                       Constants.PRISMATIC_SHARD_INDEX_I);
        }

        return true;
    }

    #endregion injected subroutines
}