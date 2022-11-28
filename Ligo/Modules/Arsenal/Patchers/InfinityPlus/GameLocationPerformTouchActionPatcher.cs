namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformTouchActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformTouchActionPatcher"/> class.</summary>
    internal GameLocationPerformTouchActionPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performTouchAction));
    }

    #region harmony patches

    /// <summary>Apply new galaxy sword conditions.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationPerformTouchActionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

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
                        typeof(GameLocationPerformTouchActionPatcher)
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
            !Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, SObject.prismaticShardIndex) ||
            Game1.player.mailReceived.Contains("galaxySword"))
        {
            return false;
        }

        return !ModEntry.Config.Arsenal.InfinityPlusOne ||
               Game1.player.hasItemInInventory(SObject.iridiumBar, 10);
    }

    #endregion injected subroutines
}
