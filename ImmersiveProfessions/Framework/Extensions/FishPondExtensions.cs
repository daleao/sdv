namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System.Linq;
using System.Reflection;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

using Common.Extensions.Reflection;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
public static class FishPondExtensions
{
    private static readonly FieldInfo _FishPondData = typeof(FishPond).RequireField("_fishPondData")!;

    /// <summary>Whether the instance's population has been fully unlocked.</summary>
    public static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var fishPondData = (FishPondData) _FishPondData.GetValue(pond);
        return fishPondData?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= fishPondData.PopulationGates.Keys.Max();
    }

    /// <summary>Whether a legendary fish lives in this pond.</summary>
    public static bool IsLegendaryPond(this FishPond pond)
    {
        return pond.GetFishObject().HasContextTag("fish_legendary");
    }
}