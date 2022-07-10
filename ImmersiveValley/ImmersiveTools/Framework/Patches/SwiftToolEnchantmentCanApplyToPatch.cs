namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SwiftToolEnchantmentCanApplyToPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SwiftToolEnchantmentCanApplyToPatch()
    {
        Target = RequireMethod<SwiftToolEnchantment>(nameof(SwiftToolEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Swift enchant to Watering Can.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool SwiftToolEnchantmentCanApplyToPrefix(ref bool __result, Item item)
    {
        __result = item is Tool tool && (tool is Axe or Hoe or Pickaxe ||
                                         tool is WateringCan &&
                                         ModEntry.Config.WateringCanConfig.AllowSwiftEnchantment);
        return false; // don't run original logic
    }

    #endregion harmony patches
}