namespace DaLion.Overhaul.Modules.Weapons.Patchers.Dwarvish;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CraftingRecipeCtorPatcher"/> class.</summary>
    internal CraftingRecipeCtorPatcher()
    {
        this.Target = this.RequireConstructor<CraftingRecipe>(typeof(string), typeof(bool));
    }

    #region harmony patches

    /// <summary>Remove Dragon Tooth from Warp Totem recipe.</summary>
    [HarmonyPostfix]
    private static void CraftingRecipeCtorPostfix(CraftingRecipe __instance)
    {
        if (WeaponsModule.Config.DwarvishLegacy && __instance.name == "Warp Totem: Island" &&
            __instance.recipeList.Remove(ItemIDs.DragonTooth))
        {
            __instance.recipeList[ItemIDs.RadioactiveOre] = 1;
        }
    }

    #endregion harmony patches
}
