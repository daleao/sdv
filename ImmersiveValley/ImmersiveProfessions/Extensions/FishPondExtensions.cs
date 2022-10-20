namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System.Linq;
using DaLion.Common.Extensions.Reflection;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
public static class FishPondExtensions
{
    private static readonly Lazy<Func<FishPond, FishPondData?>> GetFishPondData = new(() =>
        typeof(FishPond)
            .RequireField("_fishPondData")
            .CompileUnboundFieldGetterDelegate<FishPond, FishPondData?>());

    /// <summary>Determines whether the <paramref name="pond"/>'s population has been fully unlocked.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the last unlocked population gate matches the last gate in the <see cref="FishPondData"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var fishPondData = GetFishPondData.Value(pond);
        return fishPondData?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= fishPondData.PopulationGates.Keys.Max();
    }

    /// <summary>Determines whether a legendary fish lives in this <paramref name="pond"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="pond"/> houses a species of legendary fish, otherwise <see langword="false"/>.</returns>
    public static bool IsLegendaryPond(this FishPond pond)
    {
        return pond.GetFishObject().HasContextTag("fish_legendary");
    }
}
