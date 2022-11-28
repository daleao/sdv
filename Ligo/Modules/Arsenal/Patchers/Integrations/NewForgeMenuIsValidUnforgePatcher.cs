namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Menus;
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
    private static void NewForgeMenuIsValidUnforgePostfix(IClickableMenu __instance, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        var item = ModEntry.Reflector
            .GetUnboundFieldGetter<IClickableMenu, ClickableTextureComponent>(__instance, "leftIngredientSpot")
            .Invoke(__instance).item;
        __result = item is MeleeWeapon { InitialParentTileIndex: Constants.HolyBladeIndex } ||
                   (item is Slingshot slingshot && slingshot.GetTotalForgeLevels() > 0);
    }

    #endregion harmony patches
}
