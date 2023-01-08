namespace DaLion.Overhaul.Modules.Arsenal.Events.Weapons;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ComboButtonReleasedEvent : ButtonReleasedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ComboButtonReleasedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ComboButtonReleasedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnButtonReleasedImpl(object? sender, ButtonReleasedEventArgs e)
    {
        if (!e.Button.IsUseToolButton())
        {
            return;
        }

        ArsenalModule.State.HoldingWeaponSwing = false;
        this.Disable();
    }
}
