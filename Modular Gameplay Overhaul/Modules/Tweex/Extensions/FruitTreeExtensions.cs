namespace DaLion.Overhaul.Modules.Tweex.Extensions;

#region using directives

using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Tree"/> class.</summary>
internal static class FruitTreeExtensions
{
    /// <summary>Gets an object quality value based on this <paramref name="fruitTree"/> age.</summary>
    /// <param name="fruitTree">The <see cref="FruitTree"/>.</param>
    /// <returns>A <see cref="SObject"/> quality value.</returns>
    internal static int GetQualityFromAge(this FruitTree fruitTree)
    {
        var skillFactor = 1f + (Game1.player.FarmingLevel * 0.1f);
        if (Config.EnableProfessions && Game1.player.professions.Contains(Farmer.lumberjack))
        {
            skillFactor++;
        }

        var age = fruitTree.daysUntilMature.Value < 0 ? fruitTree.daysUntilMature.Value * -1 : 0;
        age = (int)(age * skillFactor * TweexModule.Config.FruitTreeAgingFactor);
        if (TweexModule.Config.DeterministicAgeQuality)
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
