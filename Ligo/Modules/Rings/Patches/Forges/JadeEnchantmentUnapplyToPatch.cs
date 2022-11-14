namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class JadeEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="JadeEnchantmentUnapplyToPatch"/> class.</summary>
    internal JadeEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<JadeEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Jade chord.</summary>
    [HarmonyPostfix]
    private static void JadeEnchantmentUnpplyToPostfix(JadeEnchantment __instance, Item item)
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
            if (chord is not null && chord.Root == Gemstone.Jade)
            {
                tool.Increment(DataFields.ResonantCritPower, __instance.GetLevel() * -0.25f);
            }
        }
    }

    #endregion harmony patches
}
