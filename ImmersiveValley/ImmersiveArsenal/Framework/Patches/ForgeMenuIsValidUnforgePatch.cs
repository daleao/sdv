namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuIsValidUnforgePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ForgeMenuIsValidUnforgePatch()
    {
        Target = RequireMethod<ForgeMenu>(nameof(ForgeMenu.IsValidUnforge));
    }

    #region harmony patches

    /// <summary>Allow unforge Holy Blade.</summary>
    [HarmonyPostfix]
    private static void ForgeMenuIsValidUnforgePostfix(ForgeMenu __instance, ref bool __result)
    {
        __result = __instance.leftIngredientSpot.item switch
        {
            Slingshot slingshot when slingshot.GetTotalForgeLevels() > 0 => true,
            MeleeWeapon { InitialParentTileIndex: Constants.HOLY_BLADE_INDEX_I } => true,
            _ => __result
        };
    }

    #endregion harmony patches
}