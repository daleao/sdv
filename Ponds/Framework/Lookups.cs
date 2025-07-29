namespace DaLion.Ponds.Framework;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>Holds maps which may be referenced by different modules.</summary>
internal static class Lookups
{
    /// <summary>Gets a map from a legendary fish ID to that of its corresponding extended family pair.</summary>
    internal static Dictionary<string, string> FamilyPairs { get; } = new()
    {
        { QIDs.Crimsonfish, QIDs.SonOfCrimsonfish },
        { QIDs.Angler, QIDs.MsAngler },
        { QIDs.Legend, QIDs.LegendII },
        { QIDs.MutantCarp, QIDs.RadioactiveCarp },
        { QIDs.Glacierfish, QIDs.GlacierfishJr },
        { QIDs.SonOfCrimsonfish, QIDs.Crimsonfish },
        { QIDs.MsAngler, QIDs.Angler },
        { QIDs.LegendII, QIDs.Legend },
        { QIDs.RadioactiveCarp, QIDs.MutantCarp },
        { QIDs.GlacierfishJr, QIDs.Glacierfish },
        { "(O)MNF.MoreNewFish_tui", "(O)MNF.MoreNewFish_la" },
        { "(O)MNF.MoreNewFish_la", "(O)MNF.MoreNewFish_tui" },
    };
}
