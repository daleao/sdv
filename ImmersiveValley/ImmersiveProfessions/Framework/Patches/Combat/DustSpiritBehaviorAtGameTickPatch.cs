namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;
using Ultimates;

#endregion using directives

[UsedImplicitly]
internal sealed class DustSpiritBehaviorAtGameTickPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal DustSpiritBehaviorAtGameTickPatch()
    {
        Target = RequireMethod<DustSpirit>(nameof(DustSpirit.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher from Dust Spirits during Ultimate.</summary>
    [HarmonyPostfix]
    private static void DustSpiritBehaviorAtGameTickPostfix(DustSpirit __instance, ref bool ___seenFarmer)
    {
        if (!__instance.Player.IsLocalPlayer || ModEntry.PlayerState.RegisteredUltimate is not
                Ambush { IsActive: true }) return;
        ___seenFarmer = false;
    }

    #endregion harmony patches
}