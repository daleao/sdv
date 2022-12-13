namespace DaLion.Overhaul.Modules.Tweex.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Gets an object quality value based on this <paramref name="object"/>'s age.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns>A <see cref="SObject"/> quality value.</returns>
    internal static int GetQualityFromAge(this SObject @object)
    {
        var skillFactor = 1f + (Game1.player.FarmingLevel * 0.1f);
        var age = (int)(@object.Read<int>(DataFields.Age) * skillFactor * (@object.IsBeeHouse()
            ?
            TweexModule.Config.BeeHouseAgingFactor
            : @object.IsMushroomBox()
                ? TweexModule.Config.MushroomBoxAgingFactor
                : 1f));

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
