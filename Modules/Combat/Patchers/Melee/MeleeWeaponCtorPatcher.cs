namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

using DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Shared.Extensions;
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
        if (__instance.ShouldBeStabbySword())
        {
            __instance.type.Value = MeleeWeapon.stabbingSword;
            Log.D($"The type of {__instance.Name} was changed to Stabbing sword.");
        }

        __instance.AddIntrinsicEnchantments();
        __instance.MakeSpecialIfNecessary();
    }

    #endregion harmony patches
}
