namespace DaLion.Redux.Framework.Arsenal.Slingshots.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="TopazEnchantmentUnapplyToPatch"/> class.</summary>
    internal TopazEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<TopazEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchant.</summary>
    [HarmonyPrefix]
    private static bool TopazEnchantmentUnapplyToPrefix(TopazEnchantment __instance, Item item)
    {
        if (item is not Slingshot)
        {
            return true; // run original logic
        }

        Game1.player.resilience -= __instance.GetLevel();

        return false; // don't run original logic
    }

    #endregion harmony patches
}
