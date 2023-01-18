namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AquamarineEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AquamarineEnchantmentApplyToPatcher"/> class.</summary>
    internal AquamarineEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<AquamarineEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Resonate with Aquamarine chord.</summary>
    [HarmonyPostfix]
    private static void AquamarineEnchantmentApplyToPostfix(Item item)
    {
        var player = Game1.player;
        if (!ArsenalModule.IsEnabled || item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var chord = player
            .Get_ResonatingChords()
            .Where(c => c.Root == Gemstone.Aquamarine)
            .ArgMax(c => c.Amplitude);
        if (chord is null)
        {
            return;
        }

        tool.UpdateResonatingChord<AquamarineEnchantment>(chord);
    }

    #endregion harmony patches
}
