namespace DaLion.Overhaul.Modules.Enchantments.Events;

#region using directives

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

        if (EnchantmentsModule.State.DoublePressTimer > 0)
        {
            EnchantmentsModule.State.GatlingModeEngaged = true;
            this.Manager.Enable<GatlingButtonReleasedEvent>();
        }
        else
        {
            EnchantmentsModule.State.DoublePressTimer = 18;
            this.Manager.Enable<GatlingUpdateTickedEvent>();
        }
    }
}
