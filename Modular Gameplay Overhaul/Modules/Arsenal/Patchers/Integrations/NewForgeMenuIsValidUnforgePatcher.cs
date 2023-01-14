namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidUnforgePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidUnforgePatcher"/> class.</summary>
    internal NewForgeMenuIsValidUnforgePatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.IsValidUnforge));
    }

    #region harmony patches

    /// <summary>Allow unforge Slingshot.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidUnforgePostfix(NewForgeMenu __instance, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        var item =__instance.leftIngredientSpot.item;
        __result = item is MeleeWeapon { InitialParentTileIndex: Constants.HolyBladeIndex } ||
                   (item is Slingshot slingshot && slingshot.GetTotalForgeLevels() > 0);
    }

    #endregion harmony patches
}
