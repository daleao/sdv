namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using System.Linq;
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

        if (WeaponsModule.Config.EnableStabbySwords &&
            (Collections.StabbingSwords.Contains(__instance.InitialParentTileIndex) ||
            WeaponsModule.Config.CustomStabbingSwords.Contains(__instance.Name)))
        {
            __instance.type.Value = MeleeWeapon.stabbingSword;
            Log.D($"The type of {__instance.Name} was converted to Stabbing sword.");
        }

        __instance.AddIntrinsicEnchantments();
        if (__instance.IsUnique() || (WeaponsModule.Config.DwarvishLegacy && __instance.CanBeCrafted()) ||
            !WeaponsModule.Config.EnableRebalance || WeaponTier.GetFor(__instance) <= WeaponTier.Untiered)
        {
            return;
        }

        if (__instance.HasIntrinsicEnchantment())
        {
            __instance.RandomizeDamage(2d);
        }
        else
        {
            __instance.RandomizeDamage();
        }
    }

    #endregion harmony patches
}
