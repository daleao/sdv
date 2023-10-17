namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using Buff = DaLion.Shared.Enums.Buff;

#endregion using directives

[UsedImplicitly]
internal sealed class YobaRemoveUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="YobaRemoveUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal YobaRemoveUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!Game1.buffsDisplay.hasBuff((int)Buff.YobasBlessing))
        {
            CombatModule.State.YobaShieldHealth = 0;
            Log.D("[CMBT]: Yoba Shield's timer has run out.");
            this.Disable();
            return;
        }

        if (Game1.player.health < Game1.player.maxHealth * 0.5)
        {
            return;
        }

        CombatModule.State.YobaShieldHealth = 0;
        Log.D("[CMBT]: Player's health reached half or above. Yoba Shield was removed.");
        this.Disable();
    }
}
