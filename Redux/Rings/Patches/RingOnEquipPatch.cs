namespace DaLion.Redux.Rings.Patches;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnEquipPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RingOnEquipPatch"/> class.</summary>
    internal RingOnEquipPatch()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onEquip));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnEquipPrefix(Ring __instance, Farmer who)
    {
        if (ModEntry.Config.Rings.TheOneIridiumBand &&
            __instance.indexInTileSheet.Value == Constants.IridiumBandIndex)
        {
            return false; // don't run original logic
        }

        if (!ModEntry.Config.Rings.RebalancedRings)
        {
            return true; // run original logic
        }

        switch (__instance.indexInTileSheet.Value)
        {
            case Constants.TopazRingIndex: // topaz to give defense
                who.resilience += 3;
                return false; // don't run original logic
            case Constants.JadeRingIndex: // jade ring to give +50% crit. power
                who.critPowerModifier += 0.5f;
                return false; // don't run original logic
            case Constants.CrabRingIndex: // crab ring to give +10 defense
                who.resilience += 10;
                return false; // don't run original logic
            default:
                if (__instance.ParentSheetIndex != Globals.GarnetRingIndex)
                {
                    return true; // run original logic
                }

                // garnet ring to give +10% cdr
                who.Increment(DataFields.CooldownReduction);
                return false; // don't run original logic
        }
    }

    #endregion harmony patches
}
