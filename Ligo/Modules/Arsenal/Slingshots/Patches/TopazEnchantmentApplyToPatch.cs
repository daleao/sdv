namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentApplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="TopazEnchantmentApplyToPatch"/> class.</summary>
    internal TopazEnchantmentApplyToPatch()
    {
        this.Target = this.RequireMethod<TopazEnchantment>("_ApplyTo");
        this.Prefix!.before = new[] { "DaLion.Arsenal" };
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchant.</summary>
    [HarmonyPrefix]
    [HarmonyBefore("DaLion.Arsenal")]
    private static bool TopazEnchantmentApplyToPrefix(TopazEnchantment __instance, Item item)
    {
        if (item is not Slingshot)
        {
            return true; // run original logic
        }

        Game1.player.resilience += __instance.GetLevel();

        return false; // don't run original logic
    }

    #endregion harmony patches
}
