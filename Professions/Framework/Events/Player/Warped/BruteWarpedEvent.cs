namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Core;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="BruteWarpedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class BruteWarpedEvent(EventManager? manager = null)
    : WarpedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.BruteRageCounter > 0;

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.NewLocation.IsEnemyArea() && !CoreMod.State.AreEnemiesNearby)
        {
            State.BruteRageCounter = 0;
        }
    }
}
