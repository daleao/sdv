namespace DaLion.Arsenal.Framework.Patchers.Infinity;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuIsValidUnforgePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuIsValidUnforgePatcher"/> class.</summary>
    internal ForgeMenuIsValidUnforgePatcher()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.IsValidUnforge));
    }

    #region harmony patches

    /// <summary>Allow unforge Holy Blade.</summary>
    [HarmonyPostfix]
    private static void ForgeMenuIsValidUnforgePostfix(ForgeMenu __instance, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        __result = CombatModule.Config.Quests.EnableHeroQuest && __instance.leftIngredientSpot.item is MeleeWeapon { InitialParentTileIndex: WeaponIds.HolyBlade };
    }

    #endregion harmony patches
}
