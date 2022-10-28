namespace DaLion.Redux.Professions.Events.GameLoop;

#region using directives

using DaLion.Redux.Professions.Ultimates;
using DaLion.Redux.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class AmbushUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="AmbushUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal AmbushUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if ((!Game1.game1.IsActiveNoOverlay && Game1.options.pauseWhenOutOfFocus) || !Game1.shouldTimePass())
        {
            return;
        }

        var ambush = Game1.player.Get_Ultimate() as Ambush;
        if (ambush!.IsActive)
        {
            Game1.player.temporarilyInvincible = true;
        }
        else
        {
            ambush.SecondsOutOfAmbush += 1d / 60d;
            if (ambush.SecondsOutOfAmbush > 1.5d)
            {
                this.Disable();
            }
        }
    }
}
