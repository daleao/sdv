namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Patches;

#region using directives

using HarmonyLib;
using Shared.Harmony;
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
    [HarmonyPostfix]
    private static void BaseWeaponEnchantmentCanApplyToPostfix(
        BaseWeaponEnchantment __instance, ref bool __result, Item item)
    {
        if (item is not Slingshot || __instance.IsSecondaryEnchantment())
        {
            return;
        }

        __result = __instance.IsForge() && ModEntry.Config.Arsenal.Slingshots.AllowForges;
    }

    #endregion harmony patches
}
