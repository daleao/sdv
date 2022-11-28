namespace DaLion.Ligo.Modules.Rings.Patchers;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
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
    private static void GarnetEnchantmentUnapplyToPostfix(GarnetEnchantment __instance, Item item)
    {
        var player = Game1.player;
        if (item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var multiplier = player.Get_ResonatingChords().Sum(c => c.Root == Gemstone.Garnet ? -0.5f : 0f);
        tool.Increment(DataFields.ResonantCooldownReduction, __instance.GetLevel() * multiplier);
        if (ModEntry.Config.EnableArsenal)
        {
            tool.Invalidate();
        }
    }

    #endregion harmony patches
}
