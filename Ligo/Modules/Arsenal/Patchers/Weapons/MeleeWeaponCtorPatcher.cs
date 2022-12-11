namespace DaLion.Ligo.Modules.Arsenal.Patchers.Weapons;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Extensions;
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
        if (__instance.InitialParentTileIndex == Constants.InsectHeadIndex)
        {
            __instance.type.Value = MeleeWeapon.dagger;
            __instance.specialItem = true;
            return;
        }

        if (Collections.StabbySwords.Contains(__instance.InitialParentTileIndex))
        {
            __instance.type.Value = MeleeWeapon.stabbingSword;
        }

        __instance.AddIntrinsicEnchantments();
    }

    #endregion harmony patches
}
