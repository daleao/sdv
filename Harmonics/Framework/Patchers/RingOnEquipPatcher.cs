﻿namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

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
    internal RingOnEquipPatcher(Harmonizer harmonizer)
        : base(harmonizer)
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
        return __instance.QualifiedItemId != QualifiedObjectIds.IridiumBand;
    }

    #endregion harmony patches
}
