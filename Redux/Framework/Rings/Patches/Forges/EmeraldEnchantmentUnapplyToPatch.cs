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
internal sealed class EmeraldEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="EmeraldEnchantmentUnapplyToPatch"/> class.</summary>
    internal EmeraldEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<EmeraldEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Emerald chord.</summary>
    [HarmonyPostfix]
    private static void EmeraldEnchantmentUnapplyToPostfix(EmeraldEnchantment __instance, Item item)
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
            if (chord is not null && chord.Root == Gemstone.Emerald)
            {
                switch (item)
                {
                    case MeleeWeapon weapon:
                        weapon.Increment(DataFields.ResonantWeaponSpeed, __instance.GetLevel() * -0.5f);
                        break;
                    case Slingshot slingshot:
                        slingshot.Increment(DataFields.ResonantSlingshotSpeed, __instance.GetLevel() * -0.5f);
                        break;
                }
            }
        }
    }

    #endregion harmony patches
}
