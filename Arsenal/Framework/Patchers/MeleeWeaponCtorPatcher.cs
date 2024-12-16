namespace DaLion.Arsenal.Framework.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
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
        this.Target = this.RequireConstructor<MeleeWeapon>(typeof(string));
    }

    #region harmony patches

    /// <summary>Convert stabby swords + add intrinsic enchants.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCtorPostfix(MeleeWeapon __instance)
    {
        if (__instance.isScythe())
        {
            return;
        }

        __instance.AddIntrinsicEnchantments();
        __instance.MakeSpecialIfNecessary();
    }

    #endregion harmony patches
}
