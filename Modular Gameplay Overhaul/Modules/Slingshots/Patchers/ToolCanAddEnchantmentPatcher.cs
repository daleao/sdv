namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolCanAddEnchantmentPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolCanAddEnchantmentPatcher"/> class.</summary>
    internal ToolCanAddEnchantmentPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.CanAddEnchantment));
    }

    #region harmony patches

    /// <summary>Allow slingshot gemstone enchantments.</summary>
    [HarmonyPrefix]
    private static bool ToolCanAddEnchantmentPrefix(
        Tool __instance, ref bool __result, BaseEnchantment enchantment)
    {
        if (__instance is not Slingshot slingshot)
        {
            return true; // run original logic
        }

        if (enchantment is InfinityEnchantment &&
            slingshot.InitialParentTileIndex == ItemIDs.GalaxySlingshot &&
            slingshot.hasEnchantmentOfType<GalaxySoulEnchantment>() &&
            slingshot.GetEnchantmentLevel<GalaxySoulEnchantment>() >= 3 &&
            SlingshotsModule.Config.EnableInfinitySlingshot)
        {
            __result = true;
            return false; // don't run original logic
        }

        if (!enchantment.IsSecondaryEnchantment() && enchantment.IsForge() &&
            SlingshotsModule.Config.EnableEnchantments)
        {
            __result = true;
            return true; // run original logic
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
