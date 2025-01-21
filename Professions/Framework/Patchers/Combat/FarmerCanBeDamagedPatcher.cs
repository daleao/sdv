namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCanBeDamagedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCanBeDamagedPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmerCanBeDamagedPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.CanBeDamaged));
    }

    #region harmony patches

    /// <summary>Patch to make Poacher invulnerable in Limit Break.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FarmerCanBeDamagedPrefix(Farmer __instance, ref bool __result)
    {
        if (!__instance.IsAmbushing())
        {
            return true; // run original logic
        }

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
