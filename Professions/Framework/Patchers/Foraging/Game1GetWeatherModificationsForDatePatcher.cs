namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1GetWeatherModificationsForDatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1GetWeatherModificationsForDatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal Game1GetWeatherModificationsForDatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Game1>(nameof(Game1.getWeatherModificationsForDate));
    }

    #region harmony patches

    /// <summary>Patch for Prestiged Arborist Green Rain chance.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void Game1GetWeatherModificationsForDatePostfix(ref string __result)
    {
        if (__result != "Rain" || !Game1.player.HasProfession(Profession.Arborist, true))
        {
            return;
        }

        var greenRainTrees = Game1.getFarm().CountGreenRainTrees();
        if (greenRainTrees == 0)
        {
            return;
        }

        var greenRainChance = Math.Min(greenRainTrees * 0.015, 21);
        if (Game1.random.NextBool(greenRainChance))
        {
            __result = "GreenRain";
        }
    }

    #endregion harmony patches
}
