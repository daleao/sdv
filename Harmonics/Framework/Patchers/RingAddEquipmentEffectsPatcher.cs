namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

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
    internal RingAddEquipmentEffectsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.AddEquipmentEffects));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Iridium Band does nothing + rebalance Jade Ring.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingAddEquipmentEffectsPrefix(Ring __instance, BuffEffects effects)
    {
        if (__instance.QualifiedItemId != QualifiedObjectIds.JadeRing)
        {
            return __instance.QualifiedItemId != QualifiedObjectIds.IridiumBand;
        }

        effects.CriticalPowerMultiplier.Value += 0.5f;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
