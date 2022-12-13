namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Slingshots;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseWeaponEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseWeaponEnchantmentCanApplyToPatcher"/> class.</summary>
    internal BaseWeaponEnchantmentCanApplyToPatcher()
    {
        this.Target = this.RequireMethod<BaseWeaponEnchantment>("CanApplyTo");
    }

    #region harmony patches

    /// <summary>Allow Slingshot forges.</summary>
    [HarmonyPrefix]
    private static bool BaseWeaponEnchantmentCanApplyToPostfix(
        BaseWeaponEnchantment __instance, ref bool __result, Item item)
    {
        if (item is not Slingshot || __instance.IsSecondaryEnchantment())
        {
            return true; // run original logic
        }

        __result = __instance.IsForge() && ArsenalModule.Config.Slingshots.AllowForges;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
