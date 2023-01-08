namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodEnchantmentCanApplyToPatcher"/> class.</summary>
    internal FishingRodEnchantmentCanApplyToPatcher()
    {
        this.Target = this.RequireMethod<FishingRodEnchantment>(nameof(FishingRodEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Master enchantment to other tools.</summary>
    [HarmonyPostfix]
    private static void FishingRodEnchantmentCanApplyTo(FishingRodEnchantment __instance, ref bool __result, Item item)
    {
        if (__instance is not MasterEnchantment)
        {
            return;
        }

        switch (item)
        {
            case Axe when ToolsModule.Config.Axe.AllowMasterEnchantment:
            case Hoe when ToolsModule.Config.Hoe.AllowMasterEnchantment:
            case Pickaxe when ToolsModule.Config.Pick.AllowMasterEnchantment:
            case WateringCan when ToolsModule.Config.Can.AllowMasterEnchantment:
                __result = true;
                break;
        }
    }

    #endregion harmony patches
}
