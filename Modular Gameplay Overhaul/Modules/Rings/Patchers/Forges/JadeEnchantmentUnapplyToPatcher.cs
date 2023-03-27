namespace DaLion.Overhaul.Modules.Rings.Patchers.Forges;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class JadeEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="JadeEnchantmentUnapplyToPatcher"/> class.</summary>
    internal JadeEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<JadeEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Jade chord.</summary>
    [HarmonyPostfix]
    private static void JadeEnchantmentUnapplyToPostfix(Item item)
    {
        var player = Game1.player;
        if (item is not Tool tool || tool != player.CurrentTool)
        {
            return;
        }

        if ((tool is MeleeWeapon && !WeaponsModule.IsEnabled) || (tool is Slingshot && !SlingshotsModule.IsEnabled) ||
            tool is not (MeleeWeapon or Slingshot))
        {
            return;
        }

        var chord = player
            .Get_ResonatingChords()
            .Where(c => c.Root == Gemstone.Jade)
            .ArgMax(c => c.Amplitude);
        if (chord is not null && tool.Get_ResonatingChord<JadeEnchantment>() == chord)
        {
            tool.UnsetResonatingChord<JadeEnchantment>();
        }
    }

    #endregion harmony patches
}
