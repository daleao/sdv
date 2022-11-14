namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GarnetEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GarnetEnchantmentApplyToPatcher"/> class.</summary>
    internal GarnetEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<GarnetEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Resonate with Garnet chord.</summary>
    [HarmonyPostfix]
    private static void GarnetEnchantmentApplyToPostfix(GarnetEnchantment __instance, Item item)
    {
        var player = Game1.player;
        if (item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var rings = Ligo.Integrations.WearMoreRingsApi?.GetAllRings(player) ??
                    player.leftRing.Value.Collect(player.rightRing.Value);
        foreach (var ring in rings.OfType<CombinedRing>())
        {
            var chord = ring.Get_Chord();
            if (chord is not null && chord.Root == Gemstone.Garnet)
            {
                tool.Increment(DataFields.ResonantCooldownReduction, __instance.GetLevel() * 0.5f);
            }
        }
    }

    #endregion harmony patches
}
