namespace DaLion.Overhaul;

#region using directives

using DaLion.Shared.Watchers;

#endregion using directives

/// <summary>Holds global <see cref="IWatcher"/> instances.</summary>
internal static class Watchers
{
    internal static Lazy<ICollectionWatcher<int>> ProfessionsWatcher { get; } =
        new(() => WatcherFactory.ForNetIntList("professions", Game1.player.professions));
}
