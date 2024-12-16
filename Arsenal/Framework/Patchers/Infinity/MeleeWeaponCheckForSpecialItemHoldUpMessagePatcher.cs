namespace DaLion.Arsenal.Framework.Patchers.Infinity;

#region using directives

using System.Reflection;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCheckForSpecialItemHoldUpMessagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponCheckForSpecialItemHoldUpMessagePatcher"/> class.</summary>
    internal MeleeWeaponCheckForSpecialItemHoldUpMessagePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.checkForSpecialItemHoldUpMeessage));
    }

    #region harmony patches

    /// <summary>Add obtain legendary weapon messages.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponCheckForSpecialItemHoldUpPostfix(MeleeWeapon __instance, ref string? __result)
    {
        if (!CombatModule.Config.Quests.EnableHeroQuest)
        {
            return true; // run original logic
        }

        try
        {
            if (__instance.isGalaxyWeapon())
            {
                var count = Game1.player.Read(DataKeys.GalaxyArsenalObtained).ParseList<int>().Count;
                var type = (WeaponType)__instance.type.Value;
                var typeString = type is WeaponType.StabbingSword or WeaponType.DefenseSword ? "sword" : type.ToStringFast().ToLowerInvariant();
                var typeDisplayName = _I18n.Get("weapons.type." + typeString).ToString().ToLower();
                __result = count == 1
                    ? I18n.Fromcsfiles_MeleeWeapon_Cs_14122(typeDisplayName, __instance.DisplayName)
                    : null;
                return false; // don't run original logic
            }

            switch (__instance.InitialParentTileIndex)
            {
                case WeaponIds.DarkSword:
                    {
                        var darkSword = I18n.Weapons_DarkSword_Name();
                        __result = I18n.Weapons_DarkSword_HoldUpMessage(darkSword);
                        break;
                    }

                case WeaponIds.HolyBlade:
                    {
                        var darkSword = I18n.Weapons_DarkSword_Name();
                        var holyBlade = I18n.Weapons_HolyBlade_Name();
                        __result = I18n.Weapons_HolyBlade_HoldUpMessage(darkSword, holyBlade);
                        break;
                    }
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
