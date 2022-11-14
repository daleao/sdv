namespace DaLion.Ligo.Modules.Tools.Patches;

#region using directives

using HarmonyLib;
using Shared.Harmony;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SwiftToolEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SwiftToolEnchantmentCanApplyToPatcher"/> class.</summary>
    internal SwiftToolEnchantmentCanApplyToPatcher()
    {
        this.Target = this.RequireMethod<SwiftToolEnchantment>(nameof(SwiftToolEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Swift enchant to Watering Can.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool SwiftToolEnchantmentCanApplyToPrefix(ref bool __result, Item item)
    {
        __result = item is Tool tool && (tool is Axe or Hoe or Pickaxe ||
                                         (tool is WateringCan &&
                                          ModEntry.Config.Tools.Can.AllowSwiftEnchantment));
        return false; // don't run original logic
    }

    #endregion harmony patches
}
