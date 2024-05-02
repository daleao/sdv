namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Constants;
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
        Tool __instance, ref bool __result, BaseEnchantment? enchantment)
    {
        if (__instance is not Slingshot slingshot || enchantment is null ||
            ArcheryIntegration.Instance?.ModApi?.GetWeaponData(Manifest, slingshot) is not null)
        {
            return true; // run original logic
        }

        if (enchantment.IsSecondaryEnchantment() && slingshot.InitialParentTileIndex == WeaponIds.GalaxySlingshot &&
            CombatModule.Config.WeaponsSlingshots.EnableInfinitySlingshot)
        {
            switch (enchantment)
            {
                case GalaxySoulEnchantment when slingshot.GetEnchantmentLevel<GalaxySoulEnchantment>() < 3:
                    __result = true;
                    return false; // don't run original logic
                case InfinityEnchantment when slingshot.GetEnchantmentLevel<GalaxySoulEnchantment>() >= 3:
                    __result = true;
                    return false; // don't run original logic
                default:
                    __result = false;
                    return false; // don't run original logic
            }
        }

        if (!CombatModule.Config.RingsEnchantments.NewPrismaticEnchantments)
        {
            __result = false;
            return false; // don't run original logic
        }

        if (!enchantment.IsForge())
        {
            __result = true;
            return false; // don't run original logic
        }

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
