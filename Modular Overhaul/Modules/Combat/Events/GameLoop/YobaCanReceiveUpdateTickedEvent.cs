namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class YobaCanReceiveUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="YobaCanReceiveUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal YobaCanReceiveUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.player.health < Game1.player.maxHealth)
        {
            return;
        }

        CombatModule.State.CanReceiveYobaShield = true;
        Log.D("[CMBT]: Player's health fully recovered. Can now receive Yoba Shield again.");
        this.Disable();
    }
}
