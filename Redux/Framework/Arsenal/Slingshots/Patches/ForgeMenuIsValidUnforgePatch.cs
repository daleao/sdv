namespace DaLion.Redux.Framework.Arsenal.Slingshots.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuIsValidUnforgePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuIsValidUnforgePatch"/> class.</summary>
    internal ForgeMenuIsValidUnforgePatch()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.IsValidUnforge));
    }

    #region harmony patches

    /// <summary>Allow unforge Slingshots.</summary>
    [HarmonyPostfix]
    private static void ForgeMenuIsValidUnforgePostfix(ForgeMenu __instance, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        __result = __instance.leftIngredientSpot.item is Slingshot slingshot && slingshot.GetTotalForgeLevels() > 0;
    }

    #endregion harmony patches
}
