namespace DaLion.Redux.Framework.Rings.Patches;

#region using directives

using System.Linq;
using DaLion.Redux.Framework.Rings.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class AquamarineEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="AquamarineEnchantmentUnapplyToPatch"/> class.</summary>
    internal AquamarineEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<AquamarineEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Aquamarine chord.</summary>
    [HarmonyPostfix]
    private static void AquamarineEnchantmentUnapplyToPostfix(AquamarineEnchantment __instance, Item item)
    {
        var player = Game1.player;
        if (!ModEntry.Config.EnableArsenal || item != player.CurrentTool)
        {
            return;
        }

        var rings = Framework.Integrations.WearMoreRingsApi?.GetAllRings(player) ??
                    player.leftRing.Value.Collect(player.rightRing.Value);
        foreach (var ring in rings.OfType<CombinedRing>())
        {
            var chord = ring.Get_Chord();
            if (chord is not null && chord.Root == Gemstone.Aquamarine)
            {
                switch (item)
                {
                    case MeleeWeapon weapon:
                        weapon.Increment(DataFields.ResonantWeaponCritChance, __instance.GetLevel() * -0.023f);
                        break;
                    case Slingshot slingshot:
                        slingshot.Increment(DataFields.ResonantSlingshotCritChance, __instance.GetLevel() * -0.023f);
                        break;
                }
            }
        }
    }

    #endregion harmony patches
}
