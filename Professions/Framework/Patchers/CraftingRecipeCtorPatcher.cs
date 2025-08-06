namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CraftingRecipeCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CraftingRecipeCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireConstructor<CraftingRecipe>(typeof(string), typeof(bool));
    }

    #region harmony patches

    /// <summary>Patch for cheaper crafting recipes for Blaster and Tapper + double yield for Prestiged Tapper.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CraftingRecipeCtorPostfix(CraftingRecipe __instance)
    {
        if (__instance.name.ContainsAnyOf("Bomb", "Explosive") && Game1.player.HasProfession(Profession.Blaster))
        {
            __instance.numberProducedPerCraft *= 2;
        }
        else if (Game1.player.HasProfession(Profession.Tapper, true) &&
                 __instance.recipeList.Keys.Any(key => key.IsSyrupId()))
        {
            __instance.numberProducedPerCraft *= 2;
        }
    }

    #endregion harmony patches
}
