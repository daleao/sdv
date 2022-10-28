namespace DaLion.Redux.Rings.Patches;

using System.Linq;
using DaLion.Shared.Extensions;
using VirtualProperties;

#region using directives

using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class AquamarineEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="AquamarineEnchantmentUnapplyToPatch"/> class.</summary>
    internal AquamarineEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<AquamarineEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Aquamarine enchant.</summary>
    [HarmonyPostfix]
    private static void AquamarineEnchantmentUnapplyToPostfix(AquamarineEnchantment __instance, Item item)
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
            if (chord is not null && chord.Root == Gemstone.Aquamarine)
            {
                weapon.critChance.Value -= __instance.GetLevel() * 0.005f;
            }
        }
    }

    #endregion harmony patches
}
