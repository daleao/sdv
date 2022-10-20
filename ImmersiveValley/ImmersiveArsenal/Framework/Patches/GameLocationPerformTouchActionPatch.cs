namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;
using Utility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformTouchActionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformTouchActionPatch"/> class.</summary>
    internal GameLocationPerformTouchActionPatch()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performTouchAction));
    }

    #region harmony patches

    /// <summary>Apply new galaxy sword conditions.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationPerformTouchActionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, 74)
        // To: if (DoesPlayerMeetGalaxyConditions())
        //     -- and also
        // Injected: this.playSound("thunder");
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.ActiveObject))))
                .StripLabels(out var labels)
                .AdvanceUntil(new CodeInstruction(OpCodes.Brfalse))
                .GetOperand(out var didNotMeetConditions)
                .Return()
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Brtrue))
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationPerformTouchActionPatch)
                            .RequireMethod(nameof(DoesPlayerMeetGalaxyConditions))),
                    new CodeInstruction(OpCodes.Brfalse, didNotMeetConditions),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldstr, "thunder"),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocation).RequireMethod(nameof(GameLocation.playSound))));
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
            !Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, Constants.PrismaticShardIndex) ||
            Game1.player.mailReceived.Contains("galaxySword"))
        {
            return false;
        }

        return !ModEntry.Config.InfinityPlusOneWeapons ||
               Game1.player.hasItemInInventory(Constants.IridiumBarIndex, 10);
    }

    #endregion injected subroutines
}
