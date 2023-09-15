namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TrackMonstersNpcListChangedEvent : NpcListChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TrackMonstersNpcListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TrackMonstersNpcListChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnNpcListChangedImpl(object? sender, NpcListChangedEventArgs e)
    {
        if (e.IsCurrentLocation)
        {
            GlobalState.AreEnemiesAround = e.Location.characters.Any(npc => npc.IsMonster);
        }
    }
}
