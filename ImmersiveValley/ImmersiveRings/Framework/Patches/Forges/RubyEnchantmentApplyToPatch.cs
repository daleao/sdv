namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using System.Collections.Generic;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RubyEnchantmentApplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RubyEnchantmentApplyToPatch"/> class.</summary>
    internal RubyEnchantmentApplyToPatch()
    {
        this.Target = this.RequireMethod<RubyEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Ruby enchant.</summary>
    [HarmonyPostfix]
    private static void RubyEnchantmentApplyToPostfix(RubyEnchantment __instance, Item item)
    {
        
    }

    #endregion harmony patches
}
