﻿using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;
using TheLion.Stardew.Professions.Framework.SuperMode;

namespace TheLion.Stardew.Professions.Framework.Patches.Combat;

[UsedImplicitly]
internal class DustSpiritBehaviorAtGameTickPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal DustSpiritBehaviorAtGameTickPatch()
    {
        Original = RequireMethod<DustSpirit>(nameof(DustSpirit.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher from Dust Spirits during Super Mode.</summary>
    [HarmonyPostfix]
    private static void DustSpiritBehaviorAtGameTickPostfix(DustSpirit __instance, ref bool ___seenFarmer)
    {
        if (!__instance.Player.IsLocalPlayer || ModEntry.State.Value.SuperMode is not
                {Index: SuperModeIndex.Poacher, IsActive: true}) return;
        ___seenFarmer = false;
    }

    #endregion harmony patches
}