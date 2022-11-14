namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Patches;

#region using directives

using DaLion.Shared.Attributes;
using HarmonyLib;
using Shared.Harmony;
using SpaceCore.Interface;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[Integration("spacechase0.SpaceCore")]
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
        __result = item is Slingshot slingshot && slingshot.GetTotalForgeLevels() > 0;
    }

    #endregion harmony patches
}
