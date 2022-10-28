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
internal sealed class EmeraldEnchantmentApplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="EmeraldEnchantmentApplyToPatch"/> class.</summary>
    internal EmeraldEnchantmentApplyToPatch()
    {
        this.Target = this.RequireMethod<EmeraldEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Emerald enchant.</summary>
    [HarmonyPostfix]
    private static void EmeraldEnchantmentApplyToPostfix(EmeraldEnchantment __instance, Item item)
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
            if (chord is not null && chord.Root == Gemstone.Emerald)
            {
                weapon.speed.Value += __instance.GetLevel();
            }
        }
    }

    #endregion harmony patches
}
