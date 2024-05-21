namespace DaLion.Core.Framework.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="TrackMonstersNpcListChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TrackMonstersNpcListChangedEvent(EventManager? manager = null)
    : NpcListChangedEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnNpcListChangedImpl(object? sender, NpcListChangedEventArgs e)
    {
        if (e.IsCurrentLocation)
        {
            State.AreEnemiesNearby = e.Location.characters.Any(npc => npc.IsMonster);
        }
    }
}
