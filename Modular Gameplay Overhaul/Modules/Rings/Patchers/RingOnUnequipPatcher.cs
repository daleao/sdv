namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnUnequipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingOnUnequipPatcher"/> class.</summary>
    internal RingOnUnequipPatcher()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onUnequip));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnUnequipPrefix(Ring __instance, Farmer who)
    {
        if (RingsModule.Config.TheOneInfinityBand &&
            __instance.indexInTileSheet.Value == Constants.IridiumBandIndex)
        {
            return false; // don't run original logic
        }

        if (!RingsModule.Config.RebalancedRings)
        {
            return true; // run original logic
        }

        switch (__instance.indexInTileSheet.Value)
        {
            case Constants.TopazRingIndex: // topaz to give defense or cdr
                who.resilience -= 3;
                return false; // don't run original logic
            case Constants.JadeRingIndex: // jade ring to give +50% crit. power
                who.critPowerModifier -= 0.5f;
                return false; // don't run original logic
            default:
                if (!Globals.GarnetRingIndex.HasValue || __instance.ParentSheetIndex != Globals.GarnetRingIndex)
                {
                    return true; // run original logic
                }

                // garnet ring to give +10% cdr
                who.IncrementCooldownReduction(-1f);
                return false; // don't run original logic
        }
    }

    #endregion harmony patches
}
