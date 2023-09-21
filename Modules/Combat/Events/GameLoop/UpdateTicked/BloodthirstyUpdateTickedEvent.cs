namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

using DaLion.Overhaul;

#region using directives

using DaLion.Overhaul.Modules.Core.Events;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
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
    protected override void OnEnabled()
    {
        this.Manager.Enable<OutOfCombatOneSecondUpdateTickedEvent>();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (player.health <= player.maxHealth)
        {
            this.Disable();
            return;
        }

        if (Game1.game1.ShouldTimePass() && GlobalState.SecondsOutOfCombat > 25 && e.IsMultipleOf(300))
        {
            // decay counter every 5 seconds after 25 seconds out of combat
            player.health = Math.Max(player.health - Math.Max(player.maxHealth / 100, 1), player.maxHealth);
        }
    }
}
