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
internal sealed class JadeEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="JadeEnchantmentUnapplyToPatcher"/> class.</summary>
    internal JadeEnchantmentUnapplyToPatcher()
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

        var multiplier = player.Get_ResonatingChords().Sum(c => c.Root == Gemstone.Jade ? -0.25f : 0f);
        tool.Increment(DataFields.ResonantCritPower, __instance.GetLevel() * multiplier);
        if (ModEntry.Config.EnableArsenal)
        {
            tool.Invalidate();
        }
    }

    #endregion harmony patches
}
