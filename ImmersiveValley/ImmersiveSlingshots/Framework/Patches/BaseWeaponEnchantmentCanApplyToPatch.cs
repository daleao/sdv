namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseWeaponEnchantmentCanApplyToPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BaseWeaponEnchantmentCanApplyToPatch()
    {
        Target = RequireMethod<BaseWeaponEnchantment>("CanApplyTo");
    }

    #region harmony patches

    /// <summary>Allow Slingshot forges.</summary>
    [HarmonyPostfix]
    private static void BaseWeaponEnchantmentCanApplyToPostfix(BaseWeaponEnchantment __instance, ref bool __result,
        Item item)
    {
        if (item is not Slingshot || __instance.IsSecondaryEnchantment()) return;

        __result = __instance.IsForge() && ModEntry.Config.EnableSlingshotForges;
    }

    #endregion harmony patches
}