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
            .Where(c => c.Root == Gemstone.Aquamarine)
            .ArgMax(c => c.Amplitude);
        if (chord is not null)
        {
            tool.UpdateResonatingChord<AquamarineEnchantment>(chord);
        }
    }

    #endregion harmony patches
}
