namespace DaLion.Core.Framework.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TrackMonstersWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TrackMonstersWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TrackMonstersWarpedEvent(EventManager? manager = null)
        : base(manager ?? CoreMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.IsLocalPlayer)
        {
            State.AreEnemiesNearby = e.NewLocation.characters.Any(npc => npc.IsMonster);
        }
    }
}
