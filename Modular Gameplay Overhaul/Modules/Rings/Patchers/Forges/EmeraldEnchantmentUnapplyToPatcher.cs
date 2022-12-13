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
internal sealed class EmeraldEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="EmeraldEnchantmentUnapplyToPatcher"/> class.</summary>
    internal EmeraldEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<EmeraldEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Emerald chord.</summary>
    [HarmonyPostfix]
    private static void EmeraldEnchantmentUnapplyToPostfix(Item item)
    {
        var player = Game1.player;
        if (!Config.EnableArsenal || item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var chord = player.Get_ResonatingChords()
            .Where(c => c.Root == Gemstone.Emerald)
            .ArgMax(c => c.Amplitude);
        if (chord is null || tool.Get_ResonatingChord<EmeraldEnchantment>() != chord)
        {
            return;
        }

        tool.UnsetResonatingChord<EmeraldEnchantment>();
        tool.Invalidate();
    }

    #endregion harmony patches
}
