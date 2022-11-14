namespace DaLion.Ligo.Modules.Tools.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class SwiftToolEnchantmentCanApplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SwiftToolEnchantmentCanApplyToPatch"/> class.</summary>
    internal SwiftToolEnchantmentCanApplyToPatch()
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
