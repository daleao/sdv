namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;

using Ultimate;

#endregion using directives

[UsedImplicitly]
internal sealed class DustSpiritBehaviorAtGameTickPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal DustSpiritBehaviorAtGameTickPatch()
    {
        Original = RequireMethod<DustSpirit>(nameof(DustSpirit.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher from Dust Spirits during Ultimate.</summary>
    [HarmonyPostfix]
    private static void DustSpiritBehaviorAtGameTickPostfix(DustSpirit __instance, ref bool ___seenFarmer)
    {
        if (!__instance.Player.IsLocalPlayer || ModEntry.PlayerState.RegisteredUltimate is not
                Ambush {IsActive: true}) return;
        ___seenFarmer = false;
    }

    #endregion harmony patches
}