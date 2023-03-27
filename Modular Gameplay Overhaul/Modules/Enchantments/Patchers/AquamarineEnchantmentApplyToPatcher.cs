namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ImplicitIgnore]
internal sealed class AquamarineEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AquamarineEnchantmentApplyToPatcher"/> class.</summary>
    internal AquamarineEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<AquamarineEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Aquamarine enchant.</summary>
    [HarmonyPrefix]
    private static bool AquamarineEnchantmentApplyToPrefix(AquamarineEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !EnchantmentsModule.Config.RebalancedForges)
        {
            return true; // run original logic
        }

        weapon.critChance.Value += 0.046f * __instance.GetLevel();
        return false; // don't run original logic
    }

    /// <summary>Reset cached stats.</summary>
    [HarmonyPostfix]
    private static void AquamarineEnchantmentApplyPostfix(Item item)
    {
        switch (item)
        {
            case MeleeWeapon weapon when WeaponsModule.IsEnabled:
                weapon.Invalidate();
                break;
            case Slingshot slingshot when SlingshotsModule.IsEnabled:
                slingshot.Invalidate();
                break;
        }
    }

    #endregion harmony patches
}
