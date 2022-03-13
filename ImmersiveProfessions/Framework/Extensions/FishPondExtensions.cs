namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

using Common.Extensions;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
internal static class FishPondExtensions
{
    private static readonly FieldInfo _FishPondData = typeof(FishPond).Field("_fishPondData");

    /// <summary>Whether the instance's population has been fully unlocked.</summary>
    internal static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var fishPondData = (FishPondData) _FishPondData.GetValue(pond);
        return fishPondData?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= fishPondData.PopulationGates.Keys.Max();
    }

    /// <summary>Whether a legendary fish lives in this pond.</summary>
    internal static bool IsLegendaryPond(this FishPond pond)
    {
        return pond.GetFishObject().HasContextTag("fish_legendary");
    }
}