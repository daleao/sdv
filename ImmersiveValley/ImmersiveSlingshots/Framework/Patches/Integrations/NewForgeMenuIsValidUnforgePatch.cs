namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Integrations.SpaceCore;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidUnforgePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidUnforgePatch"/> class.</summary>
    internal NewForgeMenuIsValidUnforgePatch()
    {
        this.Target = "SpaceCore.Interface.NewForgeMenu"
            .ToType()
            .RequireMethod("IsValidUnforge");
    }

    #region harmony patches

    /// <summary>Allow unforge Holy Blade.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidUnforgePostfix(IClickableMenu __instance, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        var item = ExtendedSpaceCoreApi.GetNewForgeMenuLeftIngredientSpot.Value(__instance).item;
        __result = item is Slingshot slingshot && slingshot.GetTotalForgeLevels() > 0;
    }

    #endregion harmony patches
}
