namespace DaLion.Redux.Rings.Patches;

#region using directives

using System.Linq;
using DaLion.Redux.Rings.VirtualProperties;
using DaLion.Shared.Extensions;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class AmethystEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="AmethystEnchantmentUnapplyToPatch"/> class.</summary>
    internal AmethystEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<AmethystEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Amethyst enchant.</summary>
    [HarmonyPostfix]
    private static void AmethystEnchantmentUnapplyToPostfix(AmethystEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon)
        {
            return;
        }

        var player = Game1.player;
        var rings = Redux.Integrations.WearMoreRingsApi?.GetAllRings(player) ??
                    player.leftRing.Value.Collect(player.rightRing.Value);
        foreach (var ring in rings.OfType<CombinedRing>())
        {
            var chord = ring.Get_Chord();
            if (chord is not null && chord.Root == Gemstone.Amethyst)
            {
                weapon.knockback.Value -= __instance.GetLevel() * 0.5f;
            }
        }
    }

    #endregion harmony patches
}
