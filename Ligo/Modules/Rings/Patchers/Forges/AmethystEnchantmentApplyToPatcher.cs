namespace DaLion.Ligo.Modules.Rings.Patchers;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AmethystEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AmethystEnchantmentApplyToPatcher"/> class.</summary>
    internal AmethystEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<AmethystEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Resonate with Amethyst chord.</summary>
    [HarmonyPostfix]
    private static void AmethystEnchantmentApplyToPostfix(AmethystEnchantment __instance, Item item)
    {
        var player = Game1.player;
        if (!ModEntry.Config.EnableArsenal || item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var multiplier = player.Get_ResonatingChords().Sum(c => c.Root == Gemstone.Amethyst ? 0.05f : 0f);
        tool.Increment(DataFields.ResonantKnockback, __instance.GetLevel() * multiplier);
        if (ModEntry.Config.EnableArsenal)
        {
            tool.Invalidate();
        }
    }

    #endregion harmony patches
}
