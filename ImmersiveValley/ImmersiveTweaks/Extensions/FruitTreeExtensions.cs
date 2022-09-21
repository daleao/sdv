namespace DaLion.Stardew.Tweex.Extensions;

#region using directives

using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Tree"/> class.</summary>
public static class FruitTreeExtensions
{
    /// <summary>Gets an object quality value based on this <paramref name="fruitTree"/> age.</summary>
    /// <param name="fruitTree">The <see cref="FruitTree"/>.</param>
    /// <returns>A <see cref="SObject"/> quality value.</returns>
    public static int GetQualityFromAge(this FruitTree fruitTree)
    {
        var skillFactor = 1f + (Game1.player.FarmingLevel * 0.1f);
        if (ModEntry.ProfessionsApi is not null && Game1.player.professions.Contains(Farmer.lumberjack))
        {
            ++skillFactor;
        }

        var age = fruitTree.daysUntilMature.Value < 0 ? fruitTree.daysUntilMature.Value * -1 : 0;
        age = (int)(age * skillFactor * ModEntry.Config.AgeImproveQualityFactor);
        if (ModEntry.Config.DeterministicAgeQuality)
        {
            return age switch
            {
                >= 336 => SObject.bestQuality,
                >= 224 => SObject.highQuality,
                >= 112 => SObject.medQuality,
                _ => SObject.lowQuality,
            };
        }

        return Game1.random.Next(age) switch
        {
            >= 336 => SObject.bestQuality,
            >= 224 => SObject.highQuality,
            >= 112 => SObject.medQuality,
            _ => SObject.lowQuality,
        };
    }
}
