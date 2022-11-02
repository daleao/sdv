namespace DaLion.Redux.Framework.Rings.Patches;

#region using directives

using System.Linq;
using DaLion.Redux.Framework;
using DaLion.Redux.Framework.Arsenal.Weapons.Enchantments;
using DaLion.Redux.Framework.Rings.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class GarnetEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GarnetEnchantmentUnapplyToPatch"/> class.</summary>
    internal GarnetEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<GarnetEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Garnet chord.</summary>
    [HarmonyPostfix]
    private static void GarnetEnchantmentUnapplyToPostfix(GarnetEnchantment __instance, Item item)
    {
        var player = Game1.player;
        if (item != player.CurrentTool)
        {
            return;
        }

        var rings = Integrations.WearMoreRingsApi?.GetAllRings(player) ??
                    player.leftRing.Value.Collect(player.rightRing.Value);
        foreach (var ring in rings.OfType<CombinedRing>())
        {
            var chord = ring.Get_Chord();
            if (chord is not null && chord.Root == Gemstone.Garnet)
            {
                switch (item)
                {
                    case MeleeWeapon weapon:
                        weapon.Increment(DataFields.ResonantWeaponCooldownReduction, __instance.GetLevel() * -0.5f);
                        break;
                    case Slingshot slingshot:
                        slingshot.Increment(DataFields.ResonantSlingshotCooldownReduction, __instance.GetLevel() * -0.5f);
                        break;
                }
            }
        }
    }

    #endregion harmony patches
}
