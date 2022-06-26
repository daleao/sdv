namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using DaLion.Common.Extensions;
using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeCtorPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CraftingRecipeCtorPatch()
    {
        Target = RequireConstructor<CraftingRecipe>(typeof(string), typeof(bool));
    }

    #region harmony patches

    /// <summary>Patch for cheaper crafting recipes for Blaster and Tapper.</summary>
    [HarmonyPostfix]
    private static void CraftingRecipeCtorPostfix(CraftingRecipe __instance)
    {
        if (__instance.name == "Tapper" && Game1.player.HasProfession(Profession.Tapper))
            __instance.recipeList = new()
            {
                { 388, 25 }, // wood
                { 334, 1 } // copper bar
            };
        else if (__instance.name == "Heavy Tapper" && Game1.player.HasProfession(Profession.Tapper))
            __instance.recipeList = new()
            {
                { 709, 20 }, // hardwood
                { 337, 1 }, // iridium bar
                { 909, 1 } // radioactive ore
            };
        else if (__instance.name.ContainsAnyOf("Bomb", "Explosive") && Game1.player.HasProfession(Profession.Blaster))
            __instance.numberProducedPerCraft *= 2;
    }

    #endregion harmony patches
}