namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class DustSpiritBehaviorAtGameTickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DustSpiritBehaviorAtGameTickPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal DustSpiritBehaviorAtGameTickPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<DustSpirit>(nameof(DustSpirit.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher in ambush from Dust Spirits.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void DustSpiritBehaviorAtGameTickPostfix(DustSpirit __instance, ref bool ___seenFarmer)
    {
        if (!__instance.Player.IsLocalPlayer || !__instance.Player.IsAmbushing())
        {
            return;
        }

        ___seenFarmer = false;
    }

    #endregion harmony patches
}
