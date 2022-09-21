namespace DaLion.Stardew.Tweex.Extensions;

#region using directives

using DaLion.Common.Extensions.Stardew;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Determines whether the <paramref name="obj"/> is an artisan good.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is created by an artisan machine, otherwise <see langword="false"/>.</returns>
    public static bool IsArtisanGood(this SObject obj)
    {
        return obj.Category is SObject.artisanGoodsCategory or SObject.syrupCategory ||
               obj.ParentSheetIndex == 395; // exception for coffee
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a bee house.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a Bee House, otherwise <see langword="false"/>.</returns>
    public static bool IsBeeHouse(this SObject obj)
    {
        return obj.bigCraftable.Value && obj.ParentSheetIndex == 10;
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a mushroom box.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a Mushroom Box, otherwise <see langword="false"/>.</returns>
    public static bool IsMushroomBox(this SObject obj)
    {
        return obj.bigCraftable.Value && obj.ParentSheetIndex == 128;
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a gem or mineral.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a geode, gem or mineral, otherwise <see langword="false"/>.</returns>
    public static bool IsPreciousRock(this SObject obj)
    {
        return obj.Category is SObject.GemCategory or SObject.mineralsCategory;
    }

    /// <summary>Gets an object quality value based on this <paramref name="obj"/>'s age.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns>A <see cref="SObject"/> quality value.</returns>
    public static int GetQualityFromAge(this SObject obj)
    {
        var skillFactor = 1f + (Game1.player.FarmingLevel * 0.1f);
        var age = (int)(obj.Read<int>("Age") * skillFactor * ModEntry.Config.AgeImproveQualityFactor);

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
