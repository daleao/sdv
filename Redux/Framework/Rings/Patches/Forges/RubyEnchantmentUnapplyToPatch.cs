namespace DaLion.Redux.Framework.Rings.Patches;

#region using directives

using System.Linq;
using DaLion.Redux.Framework.Rings.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RubyEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RubyEnchantmentUnapplyToPatch"/> class.</summary>
    internal RubyEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<RubyEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Ruby chord.</summary>
    [HarmonyPostfix]
    private static void RubyEnchantmentUnapplyToPostfix(RubyEnchantment __instance, Item item)
    {
        var player = Game1.player;
        if (!ModEntry.Config.EnableArsenal || item != player.CurrentTool)
        {
            return;
        }

        var rings = Framework.Integrations.WearMoreRingsApi?.GetAllRings(player) ??
                    player.leftRing.Value.Collect(player.rightRing.Value);
        foreach (var ring in rings.OfType<CombinedRing>())
        {
            var chord = ring.Get_Chord();
            if (chord is null || chord.Root != Gemstone.Ruby)
            {
                continue;
            }

            switch (item)
            {
                case MeleeWeapon weapon:
                    weapon.Increment(DataFields.ResonantWeaponDamage, __instance.GetLevel() * -0.05f);
                    break;
                case Slingshot slingshot:
                    slingshot.Increment(DataFields.ResonantSlingshotDamage, __instance.GetLevel() * -0.05f);
                    break;
            }
        }
    }

    #endregion harmony patches
}
