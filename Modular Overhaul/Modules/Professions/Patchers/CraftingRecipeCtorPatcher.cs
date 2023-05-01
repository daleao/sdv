namespace DaLion.Overhaul.Modules.Professions.Patchers;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions;
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

    /// <summary>Patch for cheaper crafting recipes for Blaster and Tapper.</summary>
    [HarmonyPostfix]
    private static void CraftingRecipeCtorPostfix(CraftingRecipe __instance)
    {
        switch (__instance.name)
        {
            case "Tapper" when Game1.player.HasProfession(Profession.Tapper):
                __instance.recipeList = new Dictionary<int, int>
                {
                    { SObject.wood, 25 },
                    { SObject.copperBar, 1 },
                };
                break;
            case "Heavy Tapper" when Game1.player.HasProfession(Profession.Tapper):
                __instance.recipeList = new Dictionary<int, int>
                {
                    { ItemIDs.Hardwood, 18 },
                    { ItemIDs.RadioactiveBar, 1 },
                };
                break;
            default:
                {
                    if (__instance.name.ContainsAnyOf("Bomb", "Explosive") && Game1.player.HasProfession(Profession.Blaster))
                    {
                        __instance.numberProducedPerCraft *= 2;
                    }

                    break;
                }
        }
    }

    #endregion harmony patches
}
