namespace DaLion.Ligo.Modules.Arsenal.Patchers.Infinity;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
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

        // From: if (Game1.player.ActiveObject != null && Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, 74) && !Game1.player.mailReceived.Contains("galaxySword"))
        // To: if (DoesPlayerMeetGalaxyConditions())
        //     -- and also
        // Injected: this.playSound("thunder");
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.ActiveObject))),
                    })
                .StripLabels(out var labels)
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse) })
                .GetOperand(out var didNotMeetConditions)
                .Return()
                .Match(new[] { new CodeInstruction(OpCodes.Brtrue) }, out var count)
                .Remove(count)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocationPerformTouchActionPatcher)
                                .RequireMethod(nameof(DoesPlayerMeetGalaxyConditions))),
                        new CodeInstruction(OpCodes.Brfalse, didNotMeetConditions),
                        new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldstr, "thunder"),
                        new CodeInstruction(OpCodes.Ldc_I4_0), new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocation).RequireMethod(nameof(GameLocation.playSound))),
                    },
                    labels);
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
            !Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, SObject.prismaticShardIndex))
        {
            return false;
        }

        if (!ArsenalModule.Config.InfinityPlusOne && !Game1.player.mailReceived.Contains("galaxySword"))
        {
            return true;
        }

        if (!Game1.player.hasItemInInventory(SObject.iridiumBar, ArsenalModule.Config.IridiumBarsRequiredForGalaxyArsenal))
        {
            return false;
        }

        var obtained = Game1.player.Read(DataFields.GalaxyArsenalObtained).ParseList<int>();
        return obtained.Count < 4 && Game1.player.ActiveObject.Stack >= obtained.Count + 1;
    }

    #endregion injected subroutines
}
