namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ArtfulDashUpdateTickedEvent : UpdateTickedEvent
{
    private int _timer;

    /// <summary>Initializes a new instance of the <see cref="ArtfulDashUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArtfulDashUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._timer = 18;
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        Game1.player.DoStabbingSpecialCooldown();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (--this._timer <= 0)
        {
            this.Disable();
        }
    }
}
