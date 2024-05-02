namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicking;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SlickMovesUpdateTickingEvent : UpdateTickingEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlickMovesUpdateTickingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlickMovesUpdateTickingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickingImpl(object? sender, UpdateTickingEventArgs e)
    {
        if (this.Manager.IsEnabled<StabbingSwordSpecialUpdateTickingEvent>())
        {
            this.Disable();
            return;
        }

        var (x, y) = CombatModule.State.DriftVelocity;
        if (Math.Abs(x) < 0.1 && Math.Abs(y) < 0.1)
        {
            this.Disable();
        }

        x -= x / 16f;
        y -= y / 16f;
        var player = Game1.player;
        (player.xVelocity, player.yVelocity) = (x, y);
        CombatModule.State.DriftVelocity = new Vector2(x, y);
    }
}
