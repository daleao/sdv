namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buffs;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolAddEquipmentEffectsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolAddEquipmentEffectsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ToolAddEquipmentEffectsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.AddEquipmentEffects));
    }

    #region harmony patches

    /// <summary>Allow apply Master enchantment to other tools.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool ToolAddEquipmentEffectsPrefix(Tool __instance, BuffEffects effects)
    {
        if (!__instance.hasEnchantmentOfType<MasterEnchantment>())
        {
            return true; // run orignal logic
        }

        switch (__instance)
        {
            case Axe:
                effects.ForagingLevel.Value += 1f;
                break;
            case Pickaxe:
                effects.MiningLevel.Value += 1f;
                break;
            case Hoe:
            case WateringCan:
            case MeleeWeapon weapon when weapon.isScythe():
                effects.FarmingLevel.Value += 1f;
                break;
            case FishingRod:
                effects.FishingLevel.Value += 1f;
                break;
        }

        return false; // don't run orignal logic
    }

    #endregion harmony patches
}
