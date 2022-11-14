namespace DaLion.Ligo.Modules.Professions.Patches.Common;

#region using directives

using System.Collections.Generic;
using DaLion.Ligo.Modules.Professions.Extensions;
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
                    { Constants.HardwoodIndex, 18 },
                    { Constants.RadioactiveBarIndex, 1 },
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
