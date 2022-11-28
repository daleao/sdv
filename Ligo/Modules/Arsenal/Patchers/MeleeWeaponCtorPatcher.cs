namespace DaLion.Ligo.Modules.Arsenal.Patchers;

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
        if (!Context.IsWorldReady || __instance.isScythe())
        {
            return;
        }

        __instance.RefreshStats();
        __instance.Write(DataFields.InitialMinDamage, __instance.minDamage.Value.ToString());
        __instance.Write(DataFields.InitialMaxDamage, __instance.maxDamage.Value.ToString());
        if (ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            __instance.AddIntrinsicEnchantments();
        }
    }

    #endregion harmony patches
}
