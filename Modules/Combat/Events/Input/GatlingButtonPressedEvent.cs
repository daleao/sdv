namespace DaLion.Overhaul.Modules.Combat.Events.Input;

#region using directives

using DaLion.Overhaul.Modules.Combat.Events.GameLoop;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class GatlingButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="GatlingButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal GatlingButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!e.Button.IsUseToolButton())
        {
            return;
        }

        if (CombatModule.State.DoublePressTimer > 0)
        {
            CombatModule.State.GatlingModeEngaged = true;
            this.Manager.Enable<GatlingButtonReleasedEvent>();
        }
        else
        {
            CombatModule.State.DoublePressTimer = 18;
            this.Manager.Enable<GatlingUpdateTickedEvent>();
        }
    }
}
