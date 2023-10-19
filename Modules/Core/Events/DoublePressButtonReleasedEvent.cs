namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class DoublePressButtonReleasedEvent : ButtonReleasedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DoublePressButtonReleasedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DoublePressButtonReleasedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnButtonReleasedImpl(object? sender, ButtonReleasedEventArgs e)
    {
        // check button
        // unset flag
        this.Disable();
    }
}
