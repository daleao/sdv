namespace DaLion.Redux.Professions.Patches.Common;

#region using directives

using System.Collections.Generic;
using DaLion.Redux.Professions.Extensions;
using DaLion.Shared.Extensions;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeCtorPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CraftingRecipeCtorPatch"/> class.</summary>
    internal CraftingRecipeCtorPatch()
    {
        this.Target = this.RequireConstructor<CraftingRecipe>(typeof(string), typeof(bool));
    }

    #region harmony patches

    /// <summary>Patch for cheaper crafting recipes for Blaster and Tapper.</summary>
    [HarmonyPostfix]
    private static void CraftingRecipeCtorPostfix(CraftingRecipe __instance)
    {
        if (__instance.name == "Tapper" && Game1.player.HasProfession(Profession.Tapper))
        {
            __instance.recipeList = new Dictionary<int, int>
            {
                { 388, 25 }, // wood
                { 334, 1 }, // copper bar
            };
        }
        else if (__instance.name == "Heavy Tapper" && Game1.player.HasProfession(Profession.Tapper))
        {
            __instance.recipeList = new Dictionary<int, int>
            {
                { 709, 20 }, // hardwood
                { 337, 1 }, // iridium bar
                { 909, 1 }, // radioactive ore
            };
        }
        else if (__instance.name.ContainsAnyOf("Bomb", "Explosive") && Game1.player.HasProfession(Profession.Blaster))
        {
            __instance.numberProducedPerCraft *= 2;
        }
    }

    #endregion harmony patches
}
