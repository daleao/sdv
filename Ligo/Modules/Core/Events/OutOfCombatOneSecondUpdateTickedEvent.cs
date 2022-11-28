namespace DaLion.Ligo.Modules.Core.Events;

#region using directives

using DaLion.Ligo.Modules.Core.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class OutOfCombatOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="OutOfCombatOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal OutOfCombatOneSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        Game1.player.Set_SecondsOutOfCombat(int.MaxValue);
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        Game1.player.Increment_SecondsOutOfCombat();
    }
}
