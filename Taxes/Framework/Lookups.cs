namespace DaLion.Taxes.Framework;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>Holds maps which may be referenced by different modules.</summary>
internal static class Lookups
{
    /// <summary>Gets the qualified IDs of the Artisan machines.</summary>
    internal static HashSet<string> ArtisanMachines { get; } =
    [
        QIDs.Cask,
        QIDs.CheesePress,
        QIDs.Loom,
        QIDs.MayonnaiseMachine,
        QIDs.OilMaker,
        QIDs.PreservesJar,
        QIDs.Keg,
        QIDs.Dehydrator,
    ];
}
