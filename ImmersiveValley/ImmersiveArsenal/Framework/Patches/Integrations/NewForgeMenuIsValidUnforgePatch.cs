namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common.Attributes;
using Common.Extensions.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly, RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidUnforgePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal NewForgeMenuIsValidUnforgePatch()
    {
        Target = "SpaceCore.Interface.NewForgeMenu".ToType().RequireMethod("IsValidUnforge");
    }

    #region harmony patches

    /// <summary>Allow unforge Holy Blade.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidUnforgePostfix(IClickableMenu __instance, ref bool __result)
    {
        SpaceCoreUtils.GetNewForgeMenuLeftIngredientSpot ??= "SpaceCore.Interface.NewForgeMenu".ToType().RequireField("leftIngredientSpot")
            .CompileUnboundFieldGetterDelegate<IClickableMenu, ClickableTextureComponent>();
        var item = SpaceCoreUtils.GetNewForgeMenuLeftIngredientSpot(__instance).item;
        __result = item switch
        {
            Slingshot slingshot when slingshot.GetTotalForgeLevels() > 0 => true,
            MeleeWeapon { InitialParentTileIndex: Constants.HOLY_BLADE_INDEX_I } => true,
            _ => __result
        };
    }

    #endregion harmony patches
}