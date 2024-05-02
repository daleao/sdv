namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCooldownUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCooldownUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotCooldownUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => CombatModule.State.SlingshotCooldown > 0;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        CombatModule.State.SlingshotCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        if (CombatModule.State.SlingshotCooldown > 0)
        {
            return;
        }

        CombatModule.State.SlingshotAddedScale = 0.5f;
        Game1.playSound("objectiveComplete");
    }
}
