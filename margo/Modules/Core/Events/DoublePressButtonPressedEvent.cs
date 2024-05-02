namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class DoublePressButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DoublePressButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DoublePressButtonPressedEvent(EventManager manager)
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
            // set flag
        }
        else
        {
            CombatModule.State.DoublePressTimer = 18;
            this.Manager.Enable<DoublePressUpdateTickedEvent>();
        }
    }
}
