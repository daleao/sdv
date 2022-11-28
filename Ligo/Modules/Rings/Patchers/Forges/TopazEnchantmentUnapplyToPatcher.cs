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
    private static void TopazEnchantmentUnapplyToPostfix(TopazEnchantment __instance, Item item)
    {
        var player = Game1.player;
        if (!ModEntry.Config.EnableArsenal || item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var multiplier = player.Get_ResonatingChords().Sum(c => c.Root == Gemstone.Topaz ? -0.5f : 0f);
        tool.Increment(DataFields.ResonantResilience, __instance.GetLevel() * multiplier);
        if (ModEntry.Config.EnableArsenal)
        {
            tool.Invalidate();
        }
    }

    #endregion harmony patches
}
