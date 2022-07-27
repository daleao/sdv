namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Common.Extensions.Reflection;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using System;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
public static class FishPondExtensions
{
    private static readonly Func<FishPond, FishPondData?> _GetFishPondData = typeof(FishPond).RequireField("_fishPondData")
        .CompileUnboundFieldGetterDelegate<FishPond, FishPondData?>();

    /// <summary>Whether the instance's population has been fully unlocked.</summary>
    public static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var fishPondData = _GetFishPondData(pond);
        return fishPondData?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= fishPondData.PopulationGates.Keys.Max();
    }

    /// <summary>Whether a legendary fish lives in this pond.</summary>
    public static bool IsLegendaryPond(this FishPond pond)
    {
        return pond.GetFishObject().HasContextTag("fish_legendary");
    }
}