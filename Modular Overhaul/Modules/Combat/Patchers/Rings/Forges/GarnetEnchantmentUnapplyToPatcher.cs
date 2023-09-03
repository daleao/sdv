namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings.Forges;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Resonance;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GarnetEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GarnetEnchantmentUnapplyToPatcher"/> class.</summary>
    internal GarnetEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<GarnetEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Garnet chord.</summary>
    [HarmonyPostfix]
    private static void GarnetEnchantmentUnapplyToPostfix(Item item)
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
            .Where(c => c.Root == Gemstone.Garnet)
            .ArgMax(c => c.Amplitude);
        if (chord is not null && tool.Get_ResonatingChord<GarnetEnchantment>() == chord)
        {
            tool.UnsetResonatingChord<GarnetEnchantment>();
        }
    }

    #endregion harmony patches
}
