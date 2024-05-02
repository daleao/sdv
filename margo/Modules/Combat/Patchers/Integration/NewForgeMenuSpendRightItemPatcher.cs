namespace DaLion.Overhaul.Modules.Combat.Patchers.Integration;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuSpendRightItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuSpendRightItemPatcher"/> class.</summary>
    internal NewForgeMenuSpendRightItemPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.SpendRightItem));
    }

    #region harmony patches

    /// <summary>Prevent spending Hero Soul.</summary>
    [HarmonyPrefix]
    private static bool NewForgeMenuSpendRightItemPrefix(NewForgeMenu __instance)
    {
        try
        {
            return !JsonAssetsIntegration.HeroSoulIndex.HasValue ||
                   __instance.rightIngredientSpot.item?.ParentSheetIndex != JsonAssetsIntegration.HeroSoulIndex.Value;
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
