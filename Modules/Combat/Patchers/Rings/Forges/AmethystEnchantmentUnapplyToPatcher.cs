namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings.Forges;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Combat.Resonance;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
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
        if (item is not Tool tool || tool != player.CurrentTool)
        {
            return;
        }

        if (tool is not (MeleeWeapon or Slingshot))
        {
            return;
        }

        var chord = player
            .Get_ResonatingChords()
            .Where(c => c.Root == Gemstone.Amethyst)
            .ArgMax(c => c.Amplitude);
        if (chord is not null && tool.Get_ResonatingChord<AmethystEnchantment>() == chord)
        {
            tool.UnsetResonatingChord<AmethystEnchantment>();
        }
    }

    #endregion harmony patches
}
