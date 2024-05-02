namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Linq;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
internal static class FishPondExtensions
{
    /// <summary>Determines whether the <paramref name="pond"/>'s population has been fully unlocked.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the last unlocked population gate matches the last gate in the <see cref="FishPondData"/>, otherwise <see langword="false"/>.</returns>
    internal static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var data = Reflector
            .GetUnboundFieldGetter<FishPond, FishPondData?>("_fishPondData")
            .Invoke(pond);
        return data?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= data.PopulationGates.Keys.Max();
    }
}
