namespace DaLion.Ponds.Framework;

#region using directives

using System.Collections.Generic;

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
        QualifiedObjectIds.RadioactiveCarp,
        "MNF.MoreNewFish_mushu",
        "MNF.MoreNewFish_shinidamachu",
        "MNF.MoreNewFish_goblin_shark_pup",
        "MNF.MoreNewFish_bonnethead_pup",
        "MNF.MoreNewFish_spiny_dogfish_pup",
        "MNF.MoreNewFish_manta_ray_pup",
        "MNF.MoreNewFish_tui",
        "MNF.MoreNewFish_la",
        "MNF.MoreNewFish_iridiumfish",
    ];

    /// <summary>Gets a map from a legendary fish ID to that of its corresponding extended family pair.</summary>
    internal static Dictionary<string, string> FamilyPairs { get; } = new()
    {
        { QualifiedObjectIds.Crimsonfish, QualifiedObjectIds.SonOfCrimsonfish },
        { QualifiedObjectIds.Angler, QualifiedObjectIds.MsAngler },
        { QualifiedObjectIds.Legend, QualifiedObjectIds.LegendII },
        { QualifiedObjectIds.MutantCarp, QualifiedObjectIds.RadioactiveCarp },
        { QualifiedObjectIds.Glacierfish, QualifiedObjectIds.GlacierfishJr },
        { QualifiedObjectIds.SonOfCrimsonfish, QualifiedObjectIds.Crimsonfish },
        { QualifiedObjectIds.MsAngler, QualifiedObjectIds.Angler },
        { QualifiedObjectIds.LegendII, QualifiedObjectIds.Legend },
        { QualifiedObjectIds.RadioactiveCarp, QualifiedObjectIds.MutantCarp },
        { QualifiedObjectIds.GlacierfishJr, QualifiedObjectIds.Glacierfish },
        { "MNF.MoreNewFish_tui", "MNF.MoreNewFish_la" },
        { "MNF.MoreNewFish_la", "MNF.MoreNewFish_tui" },
    };
}
