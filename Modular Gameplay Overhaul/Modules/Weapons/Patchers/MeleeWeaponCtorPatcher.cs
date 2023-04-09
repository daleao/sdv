namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Extensions;
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

    /// <summary>Convert stabby swords + add intrinsic enchants.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCtorPostfix(MeleeWeapon __instance)
    {
        if (WeaponsModule.Config.EnableRebalance &&
            __instance.InitialParentTileIndex is ItemIDs.InsectHead or ItemIDs.NeptuneGlaive)
        {
            __instance.specialItem = true;
            if (__instance.InitialParentTileIndex == ItemIDs.InsectHead)
            {
                __instance.type.Value = MeleeWeapon.dagger;
            }

            return;
        }

        __instance.AddIntrinsicEnchantments();
        if (!__instance.ShouldBeStabbySword())
        {
            return;
        }

        __instance.type.Value = MeleeWeapon.stabbingSword;
        Log.D($"The type of {__instance.Name} was converted to Stabbing sword.");
    }

    #endregion harmony patches
}
