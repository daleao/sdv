namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class OutOfCombatWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="OutOfCombatWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal OutOfCombatWarpedEvent(EventManager? manager = null)
        : base(manager ?? CoreMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.IsDungeon() || State.AreEnemiesNearby)
        {
            this.Manager.Enable<OutOfCombatOneSecondUpdateTickedEvent>();
        }
        else
        {
            this.Manager.Disable<OutOfCombatOneSecondUpdateTickedEvent>();
        }
    }
}
