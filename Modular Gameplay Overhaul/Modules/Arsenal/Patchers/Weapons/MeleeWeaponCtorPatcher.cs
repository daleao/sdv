namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
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
        if (ArsenalModule.Config.Weapons.EnableRebalance &&
            __instance.InitialParentTileIndex is Constants.InsectHeadIndex or Constants.NeptuneGlaiveIndex)
        {
            __instance.specialItem = true;
            if (__instance.InitialParentTileIndex == Constants.InsectHeadIndex)
            {
                __instance.type.Value = MeleeWeapon.dagger;
            }

            return;
        }

        if (ArsenalModule.Config.Weapons.EnableStabbySwords &&
            (Collections.StabbingSwords.Contains(__instance.InitialParentTileIndex) ||
            ArsenalModule.Config.Weapons.CustomStabbingSwords.Contains(__instance.Name)))
        {
            __instance.type.Value = MeleeWeapon.stabbingSword;
            Log.D($"The type of {__instance.Name} was converted to Stabbing sword.");
        }

        __instance.AddIntrinsicEnchantments();
    }

    #endregion harmony patches
}
