namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Stardew.Arsenal.Extensions;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidCraftIngredient : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidCraftIngredient"/> class.</summary>
    internal NewForgeMenuIsValidCraftIngredient()
    {
        this.Target = "SpaceCore.Interface.NewForgeMenu"
            .ToType()
            .RequireMethod("IsValidCraftIngredient");
    }

    #region harmony patches

    /// <summary>Allow forging with Hero Soul.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidCraftIngredientPostfix(ref bool __result, Item item)
    {
        if (item.IsHeroSoul())
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
