namespace DaLion.Overhaul.Modules.Combat.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidCraftIngredientPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidCraftIngredientPatcher"/> class.</summary>
    internal NewForgeMenuIsValidCraftIngredientPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.IsValidCraftIngredient));
    }

    #region harmony patches

    /// <summary>Allow forging with Hero Soul.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidCraftIngredientPostfix(ref bool __result, Item item)
    {
        if (JsonAssetsIntegration.HeroSoulIndex.HasValue && item.ParentSheetIndex == JsonAssetsIntegration.HeroSoulIndex.Value)
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
