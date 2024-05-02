namespace DaLion.Ponds.Framework;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Classes;

#endregion using directives

/// <summary>Holds maps which may be referenced by different modules.</summary>
internal static class Lookups
{
    /// <summary>Gets the qualified IDs of the legendary and extended family fish.</summary>
    internal static HashSet<string> LegendaryFishes { get; } =
    [
        QualifiedObjectIds.Crimsonfish,
        QualifiedObjectIds.Angler,
        QualifiedObjectIds.Legend,
        QualifiedObjectIds.Glacierfish,
        QualifiedObjectIds.MutantCarp,
        QualifiedObjectIds.SonOfCrimsonfish,
        QualifiedObjectIds.MsAngler,
        QualifiedObjectIds.LegendII,
        QualifiedObjectIds.GlacierfishJr,
        QualifiedObjectIds.RadioactiveCarp
    ];

    /// <summary>Gets a map from a legendary fish ID to that of its corresponding extended family pair.</summary>
    internal static BiMap<string, string> FamilyPairs { get; } = new(new Dictionary<string, string>
    {
        { QualifiedObjectIds.Crimsonfish, QualifiedObjectIds.SonOfCrimsonfish },
        { QualifiedObjectIds.Angler, QualifiedObjectIds.MsAngler },
        { QualifiedObjectIds.Legend, QualifiedObjectIds.LegendII },
        { QualifiedObjectIds.MutantCarp, QualifiedObjectIds.RadioactiveCarp },
        { QualifiedObjectIds.Glacierfish, QualifiedObjectIds.GlacierfishJr },
    });
}
