namespace DaLion.Overhaul.Modules.Combat.Events.Input.ButtonReleased;

#region using directives

using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotSpecialButtonReleasedEvent : ButtonReleasedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotSpecialButtonReleasedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotSpecialButtonReleasedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => CombatModule.State.SlingshotGatlingTimer > 0;

    /// <inheritdoc />
    protected override void OnButtonReleasedImpl(object? sender, ButtonReleasedEventArgs e)
    {
        if (!e.Button.IsActionButton())
        {
            return;
        }

        this.Manager.Disable<SlingshotSpecialUpdateTickedEvent>();
        this.Disable();
    }
}
