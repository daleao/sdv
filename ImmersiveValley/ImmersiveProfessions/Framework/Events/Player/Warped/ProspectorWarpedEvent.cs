namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Professions.Extensions;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProspectorWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ProspectorWarpedEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (ModEntry.State.ProspectorHunt.Value.IsActive)
        {
            ModEntry.State.ProspectorHunt.Value.Fail();
        }

        if (!Game1.eventUp && e.NewLocation is MineShaft shaft && !shaft.IsTreasureOrSafeRoom())
        {
            ModEntry.State.ProspectorHunt.Value.TryStart(e.NewLocation);
        }
    }
}
