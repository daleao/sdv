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
internal sealed class RubyEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RubyEnchantmentApplyToPatcher"/> class.</summary>
    internal RubyEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<RubyEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Resonate with Ruby chord.</summary>
    [HarmonyPostfix]
    private static void RubyEnchantmentApplyToPostfix(Item item)
    {
        var player = Game1.player;
        if (!ArsenalModule.IsEnabled || item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var chord = player.Get_ResonatingChords()
            .Where(c => c.Root == Gemstone.Ruby)
            .ArgMax(c => c.Amplitude);
        if (chord is null)
        {
            return;
        }

        tool.UpdateResonatingChord<RubyEnchantment>(chord);
    }

    #endregion harmony patches
}
