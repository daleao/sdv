namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodOpenTreasureMenuEndFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodOpenTreasureMenuEndFunctionPatcher"/> class.</summary>
    internal FishingRodOpenTreasureMenuEndFunctionPatcher()
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.openTreasureMenuEndFunction));
    }

    #region harmony patches

    /// <summary>Prevent obtaining copies of unique fishing weapons.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishingRodOpenTreasureMenuEndFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .ForEach(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.specialItems))),
                        new CodeInstruction(OpCodes.Ldc_I4_S),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetIntList).RequireMethod(nameof(NetIntList.Contains))),
                    },
                    () =>
                        helper
                            .Advance(2)
                            .ReplaceInstructionWith(
                                new CodeInstruction(
                                    OpCodes.Call,
                                    typeof(FishingRodOpenTreasureMenuEndFunctionPatcher).RequireMethod(
                                        nameof(HasObtainedSpecialFishingWeapon)))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting unique weapon check to fishing chest reward.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool HasObtainedSpecialFishingWeapon(Farmer farmer, int index)
    {
        return farmer.Read(DataFields.UniqueWeapons).ParseList<int>().Contains(index);
    }

    #endregion injected subroutines
}
