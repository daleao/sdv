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

        var multiplier = player.Get_ResonatingChords().Sum(c => c.Root == Gemstone.Aquamarine ? -0.023f : 0f);
        tool.Increment(DataFields.ResonantCritChance, __instance.GetLevel() * multiplier);
        if (ModEntry.Config.EnableArsenal)
        {
            tool.Invalidate();
        }
    }

    #endregion harmony patches
}
