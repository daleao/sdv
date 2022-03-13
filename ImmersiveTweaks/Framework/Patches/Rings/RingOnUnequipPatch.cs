namespace DaLion.Stardew.Tweaks.Framework.Patches.Rings;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal class RingOnUnequipPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal RingOnUnequipPatch()
    {
        Original = RequireMethod<Ring>(nameof(Ring.onUnequip));
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
    [HarmonyPostfix]
    private static void RingOnUnequipPostfix(Ring __instance, Farmer who)
    {
        if (!ModEntry.Config.RebalanceRings) return;

        switch (__instance.indexInTileSheet.Value)
        {
            case 530: // topaz to give +3 defense
                who.weaponPrecisionModifier += 0.1f;
                who.resilience -= 3;
                break;
            case 532: // jade ring to give +30% crit. power
                who.critPowerModifier -= 0.2f;
                break;
            case 810: // crab ring to give +8 defense
                who.resilience -= 3;
                break;
            default:
                return;
        }
    }

    #endregion harmony patches
}