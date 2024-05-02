namespace DaLion.Overhaul;

#region using directives

using DaLion.Shared.Watchers;

#endregion using directives

/// <summary>Holds global <see cref="IWatcher"/> instances.</summary>
internal static class Watchers
{
    /// <summary>Gets a <see cref="ICollectionWatcher{TValue}"/> which monitors changes to the local player's professions.</summary>
    internal static Lazy<ICollectionWatcher<int>> ProfessionsWatcher { get; } =
        new(() => WatcherFactory.ForNetIntList("professions", Game1.player.professions));
}
