namespace DaLion.Ligo.Modules.Arsenal.Patchers.Weapons;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponCtorPatcher"/> class.</summary>
    internal MeleeWeaponCtorPatcher()
    {
        this.Target = this.RequireConstructor<MeleeWeapon>(typeof(int));
    }

    #region harmony patches

    /// <summary>Add intrinsic enchants.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCtorPostfix(MeleeWeapon __instance)
    {
        __instance.Write(DataFields.BaseMinDamage, __instance.minDamage.Value.ToString());
        __instance.Write(DataFields.BaseMaxDamage, __instance.maxDamage.Value.ToString());

        if (__instance.InitialParentTileIndex == Constants.InsectHeadIndex)
        {
            __instance.type.Value = MeleeWeapon.dagger;
            return;
        }

        if (ModEntry.Config.Arsenal.Weapons.RebalancedWeapons)
        {
            __instance.AddIntrinsicEnchantments();
        }
    }

    #endregion harmony patches
}
