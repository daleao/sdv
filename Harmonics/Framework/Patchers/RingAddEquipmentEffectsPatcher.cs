namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buffs;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingAddEquipmentEffectsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingAddEquipmentEffectsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal RingAddEquipmentEffectsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.AddEquipmentEffects));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Iridium Band does nothing + rebalance Jade Ring + implement Garnet ring.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingAddEquipmentEffectsPrefix(Ring __instance, BuffEffects effects)
    {
        if (__instance.QualifiedItemId == QIDs.JadeRing)
        {
            effects.CriticalPowerMultiplier.Value += 0.5f;
            return false; // don't run original logic
        }

        if (__instance.QualifiedItemId == GarnetRingId)
        {
            effects.Get_CooldownReduction().Value += 0.1f;
            return false; // don't run original logic
        }

        return __instance.QualifiedItemId != QIDs.IridiumBand;
    }

    #endregion harmony patches
}
