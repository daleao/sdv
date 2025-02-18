﻿namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnEquipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingOnEquipPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal RingOnEquipPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onEquip));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Iridium Band does nothing.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnEquipPrefix(Ring __instance)
    {
        if (__instance.ItemId != GarnetRingId)
        {
            return __instance.QualifiedItemId != QIDs.IridiumBand;
        }

        Game1.player.Get_CooldownReduction().Value += 0.1f;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
