namespace DaLion.Stardew.Tweex.Extensions;

#region using directives

using StardewValley;
using StardewValley.TerrainFeatures;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="Tree"/> class.</summary>
public static class FruitTreeExtensions
{
    /// <summary>Get an object quality value based on this fruit tree's age.</summary>
    public static int GetQualityFromAge(this FruitTree tree)
    {
        var skillFactor = 1f + Game1.player.FarmingLevel * 0.1f;
        var age = tree.daysUntilMature.Value < 0 ? tree.daysUntilMature.Value * -1 : 0;
        age = (int)(age * skillFactor * ModEntry.Config.AgeImproveQualityFactor);
        if (ModEntry.ProfessionsAPI is not null && Game1.player.professions.Contains(Farmer.lumberjack)) age *= 2;

        if (ModEntry.Config.DeterministicAgeQuality)
        {
            return age switch
            {
                >= 336 => SObject.bestQuality,
                >= 224 => SObject.highQuality,
                >= 112 => SObject.medQuality,
                _ => SObject.lowQuality
            };
        }

        return Game1.random.Next(age) switch
        {
            >= 336 => SObject.bestQuality,
            >= 224 => SObject.highQuality,
            >= 112 => SObject.medQuality,
            _ => SObject.lowQuality
        };
    }
}