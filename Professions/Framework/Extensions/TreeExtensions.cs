namespace DaLion.Professions.Framework.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Tree"/> class.</summary>
internal static class TreeExtensions
{
    /// <summary>Gets an object quality value based on this <paramref name="tree"/>'s age.</summary>
    /// <param name="tree">The <see cref="Tree"/>.</param>
    /// <returns>A <see cref="SObject"/> quality value.</returns>
    internal static int GetQualityFromAge(this Tree tree)
    {
        var datePlanted = Data.ReadAs<int>(tree, DataKeys.DatePlanted);
        var currentDate = Game1.game1.GetCurrentDateNumber();
        var age = currentDate - datePlanted;
        return age switch
        {
            >= 336 => SObject.bestQuality,
            >= 224 => SObject.highQuality,
            >= 112 => SObject.medQuality,
            _ => SObject.lowQuality,
        };
    }
}
