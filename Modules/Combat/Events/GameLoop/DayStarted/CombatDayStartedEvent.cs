namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.DayStarted;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class CombatDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => CombatModule.State.DidPrayToday || CombatModule.State.SpokeWithWizardToday;

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        CombatModule.State.DidPrayToday = false;
        CombatModule.State.SpokeWithWizardToday = false;
        CombatModule.State.UsedSandPillarsToday = false;
    }
}
