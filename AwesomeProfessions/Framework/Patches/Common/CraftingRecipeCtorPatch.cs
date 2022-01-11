using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
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
                { 388, 25 }, // wood
                { 334, 1 } // copper bar
            };
        else if (__instance.name == "Heavy Tapper" && Game1.player.HasProfession("Tapper"))
            __instance.recipeList = new()
            {
                { 709, 20 }, // hardwood
                { 337, 1 }, // iridium bar
                { 909, 1 } // radioactive ore
            };
        else if (__instance.name.ContainsAnyOf("Bomb", "Explosive") && Game1.player.HasProfession("Blaster"))
            __instance.numberProducedPerCraft *= 2;
    }

    #endregion harmony patches
}