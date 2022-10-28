namespace DaLion.Redux.Arsenal.Slingshots.Patches;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidUnforgePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidUnforgePatch"/> class.</summary>
    internal NewForgeMenuIsValidUnforgePatch()
    {
        this.Target = typeof(SpaceCore.Interface.NewForgeMenu)
                .RequireMethod(nameof(SpaceCore.Interface.NewForgeMenu.IsValidUnforge));
    }

    #region harmony patches

    /// <summary>Allow unforge Slingshot.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidUnforgePostfix(IClickableMenu __instance, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        var item = ModEntry.Reflector
            .GetUnboundFieldGetter<IClickableMenu, ClickableTextureComponent>(__instance, "leftIngredientSpot")
            .Invoke(__instance).item;
        __result = item is Slingshot slingshot && slingshot.GetTotalForgeLevels() > 0;
    }

    #endregion harmony patches
}
