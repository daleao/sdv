namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AmethystEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AmethystEnchantmentUnapplyToPatcher"/> class.</summary>
    internal AmethystEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<AmethystEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Amethyst chord.</summary>
    [HarmonyPostfix]
    private static void AmethystEnchantmentUnapplyToPostfix(Item item)
    {
        var player = Game1.player;
        if (!Config.EnableArsenal || item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var chord = player.Get_ResonatingChords()
            .Where(c => c.Root == Gemstone.Amethyst)
            .ArgMax(c => c.Amplitude);
        if (chord is null || tool.Get_ResonatingChord<AmethystEnchantment>() != chord)
        {
            return;
        }

        tool.UnsetResonatingChord<AmethystEnchantment>();
        tool.Invalidate();
    }

    #endregion harmony patches
}
