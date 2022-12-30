namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
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

    /// <summary>Reset cached stats.</summary>
    [HarmonyPostfix]
    private static void AquamarineEnchantmentUnapplyPostfix(Item item)
    {
        if (item is Tool tool and (MeleeWeapon or Slingshot))
        {
            tool.Invalidate();
        }
    }

    #endregion harmony patches
}
