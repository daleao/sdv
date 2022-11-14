namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
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
        if (!ModEntry.Config.EnableArsenal || item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var rings = Ligo.Integrations.WearMoreRingsApi?.GetAllRings(player) ??
                    player.leftRing.Value.Collect(player.rightRing.Value);
        foreach (var ring in rings.OfType<CombinedRing>())
        {
            var chord = ring.Get_Chord();
            if (chord is not null && chord.Root == Gemstone.Ruby)
            {
                tool.Increment(DataFields.ResonantDamage, __instance.GetLevel() * -0.05f);
            }
        }
    }

    #endregion harmony patches
}
