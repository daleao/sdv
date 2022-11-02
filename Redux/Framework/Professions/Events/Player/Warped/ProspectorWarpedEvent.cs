namespace DaLion.Redux.Framework.Professions.Events.Player;

#region using directives

using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProspectorWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (ModEntry.State.Professions.ProspectorHunt.Value.IsActive)
        {
            ModEntry.State.Professions.ProspectorHunt.Value.Fail();
        }

        if (e.NewLocation is MineShaft shaft && !shaft.IsTreasureOrSafeRoom())
        {
            ModEntry.State.Professions.ProspectorHunt.Value.TryStart(e.NewLocation);
        }
    }
}
