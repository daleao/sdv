namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class AmbushUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="AmbushUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal AmbushUpdateTickedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!Game1.game1.ShouldTimePass())
        {
            return;
        }

        if (State.LimitBreak is not PoacherAmbush ambush)
        {
            this.Disable();
            return;
        }

        if (ambush.IsActive)
        {
            Game1.player.temporarilyInvincible = true;
        }
        else
        {
            ambush.SecondsOutOfAmbush += 1d / 60d;
            if (ambush.SecondsOutOfAmbush > 1d)
            {
                this.Disable();
            }
        }
    }
}
