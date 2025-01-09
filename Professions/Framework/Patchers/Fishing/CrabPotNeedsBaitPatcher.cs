namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotNeedsBaitPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotNeedsBaitPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CrabPotNeedsBaitPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.NeedsBait));
    }

    #region harmony patches

    /// <summary>Patch to allow Conservationist to place bait.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool CrabPotNeedsBaitPostfix(CrabPot __instance, ref bool __result)
    {
        __result = __instance.bait.Value == null;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
