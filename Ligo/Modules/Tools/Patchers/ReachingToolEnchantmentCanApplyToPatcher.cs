namespace DaLion.Ligo.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ReachingToolEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ReachingToolEnchantmentCanApplyToPatcher"/> class.</summary>
    internal ReachingToolEnchantmentCanApplyToPatcher()
    {
        this.Target = this.RequireMethod<ReachingToolEnchantment>(nameof(ReachingToolEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Reaching enchant to Axe and Pick.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool ReachingToolEnchantmentCanApplyToPrefix(ref bool __result, Item item)
    {
        if (item is Tool tool && (tool is WateringCan or Hoe ||
                                  (tool is Axe && ModEntry.Config.Tools.Axe.AllowReachingEnchantment) ||
                                  (tool is Pickaxe && ModEntry.Config.Tools.Pick.AllowReachingEnchantment)))
        {
            __result = tool.UpgradeLevel == 4;
        }
        else
        {
            __result = false;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
