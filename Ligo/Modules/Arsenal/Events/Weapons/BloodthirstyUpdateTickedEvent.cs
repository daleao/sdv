namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class BloodthirstyUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="BloodthirstyUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BloodthirstyUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (player.health <= player.maxHealth ||
            (!Game1.game1.IsActiveNoOverlay && Game1.options.pauseWhenOutOfFocus) || !Game1.shouldTimePass() ||
            !e.IsOneSecond)
        {
            return;
        }

        ++ModEntry.State.Arsenal.SecondsOutOfCombat;
        // decay counter every 5 seconds after 25 seconds out of combat
        if (ModEntry.State.Arsenal.SecondsOutOfCombat > 30 && e.IsMultipleOf(300))
        {
            player.health = Math.Max(player.health - Math.Max(player.maxHealth / 100, 1), player.maxHealth);
        }
    }
}
