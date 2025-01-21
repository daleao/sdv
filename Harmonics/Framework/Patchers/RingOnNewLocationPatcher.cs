﻿namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnNewLocationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingOnNewLocationPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal RingOnNewLocationPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onNewLocation));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Iridium Band does nothing.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnNewLocationPrefix(Ring __instance)
    {
        return __instance.QualifiedItemId != QIDs.IridiumBand;
    }

    #endregion harmony patches
}
