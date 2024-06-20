namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings.Resonance;

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
internal sealed class TopazEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TopazEnchantmentUnapplyToPatcher"/> class.</summary>
    internal TopazEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<TopazEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Topaz chord.</summary>
    [HarmonyPostfix]
    private static void TopazEnchantmentUnapplyToPostfix(Item item)
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
            .Where(c => c.Root == Gemstone.Topaz)
            .ArgMax(c => c.Amplitude);
        if (chord is not null && tool.Get_ResonatingChord<TopazEnchantment>() == chord)
        {
            tool.UnsetResonatingChord<TopazEnchantment>();
        }
    }

    #endregion harmony patches
}
