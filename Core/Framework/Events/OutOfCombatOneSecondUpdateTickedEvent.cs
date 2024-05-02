namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class OutOfCombatOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="OutOfCombatOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal OutOfCombatOneSecondUpdateTickedEvent(EventManager? manager = null)
        : base(manager ?? CoreMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        State.SecondsOutOfCombat = 0;
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        State.SecondsOutOfCombat = int.MaxValue;
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (++State.SecondsOutOfCombat > 300)
        {
            this.Disable();
        }
    }
}
