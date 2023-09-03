namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCanBeDamagedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCanBeDamagedPatcher"/> class.</summary>
    internal FarmerCanBeDamagedPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.CanBeDamaged));
    }

    #region harmony patches

    /// <summary>Ring of Yoba rebalance.</summary>
    [HarmonyPrefix]
    private static bool FarmerCanBeDamagedPostfix(Farmer __instance, ref bool __result)
    {
        __result = !__instance.temporarilyInvincible && !__instance.isEating && !Game1.fadeToBlack &&
               (!Game1.buffsDisplay.hasBuff(21) || CombatModule.Config.RebalancedRings);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
