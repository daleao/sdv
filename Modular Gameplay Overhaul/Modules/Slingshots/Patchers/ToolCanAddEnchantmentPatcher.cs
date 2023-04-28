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

    /// <summary>Allow slingshot enchantments.</summary>
    [HarmonyPrefix]
    private static bool ToolCanAddEnchantmentPrefix(
        Tool __instance, ref bool __result, BaseEnchantment enchantment)
    {
        if (__instance is not Slingshot slingshot)
        {
            return true; // run original logic
        }

        if (slingshot.InitialParentTileIndex == ItemIDs.GalaxySlingshot &&
            SlingshotsModule.Config.EnableInfinitySlingshot)
        {
            switch (enchantment)
            {
                case GalaxySoulEnchantment when slingshot.GetEnchantmentLevel<GalaxySoulEnchantment>() < 3:
                    __result = true;
                    return false; // don't run original logic
                case InfinityEnchantment when slingshot.GetEnchantmentLevel<GalaxySoulEnchantment>() >= 3:
                    __result = true;
                    return false; // don't run original logic
            }
        }

        if (enchantment.IsSecondaryEnchantment())
        {
            __result = false;
            return false; // don't run original logic
        }

        if (enchantment.IsForge())
        {
            __result = SlingshotsModule.Config.EnableEnchantments;
            return false; // don't run original logic
        }

        return EnchantmentsModule.ShouldEnable && EnchantmentsModule.Config.RangedEnchantments;
    }

    #endregion harmony patches
}
