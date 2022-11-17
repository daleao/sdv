namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AquamarineEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AquamarineEnchantmentUnapplyToPatcher"/> class.</summary>
    internal AquamarineEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<AquamarineEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Aquamarine chord.</summary>
    [HarmonyPostfix]
    private static void AquamarineEnchantmentUnapplyToPostfix(AquamarineEnchantment __instance, Item item)
    {
        var player = Game1.player;
        if (!ModEntry.Config.EnableArsenal || item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var rings = Ligo.Integrations.WearMoreRingsApi?.GetAllRings(player) ??
                    player.leftRing.Value.Collect(player.rightRing.Value);
        foreach (var ring in rings.OfType<CombinedRing>())
        {
            var chord = ring.Get_Chord();
            if (chord is not null && chord.Root == Gemstone.Aquamarine)
            {
                tool.Increment(DataFields.ResonantCritChance, __instance.GetLevel() * -0.023f);
            }
        }
    }

    #endregion harmony patches
}
