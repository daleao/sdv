namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProspectorHuntDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorHuntDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Prospector);

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        Game1.player.Get_ProspectorHunt().ResetChanceAccumulator();
    }
}
