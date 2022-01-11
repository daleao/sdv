using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches.Common;

[UsedImplicitly]
internal class CraftingRecipeCtorPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CraftingRecipeCtorPatch()
    {
        Original = RequireConstructor<CraftingRecipe>(typeof(string), typeof(bool));
    }

    #region harmony patches

    /// <summary>Patch for cheaper crafting recipes for Blaster and Tapper.</summary>
    [HarmonyPostfix]
    private static void CraftingRecipeCtorPostfix(ref CraftingRecipe __instance)
    {
        if (__instance.name == "Tapper" && Game1.player.HasProfession("Tapper"))
            __instance.recipeList = new()
            {
                {388, 25}, // wood
                {334, 1} // copper bar
            };
        else if (__instance.name == "Heavy Tapper" && Game1.player.HasProfession("Tapper"))
            __instance.recipeList = new()
            {
                {709, 20}, // hardwood
                {337, 1}, // iridium bar
                {909, 1} // radioactive ore
            };
        else if (__instance.name.Contains("Bomb") && Game1.player.HasProfession("Blaster"))
            __instance.recipeList = __instance.name switch
            {
                "Cherry Bomb" => new()
                {
                    {378, 2}, // copper ore
                    {382, 1} // coal
                },
                "Bomb" => new()
                {
                    {380, 2}, // iron ore
                    {382, 1} // coal
                },
                "Mega Bomb" => new()
                {
                    {384, 2}, // gold ore
                    {382, 1} // coal
                },
                "Explosive Ammo" => new()
                {
                    {380, 3}, // iron ore
                    {382, 1} // coal
                },
                _ => __instance.recipeList
            };
    }

    #endregion harmony patches
}