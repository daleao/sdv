namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class BlueprintDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="BlueprintDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BlueprintDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        Game1.player.Write(DataFields.DaysLeftTranslating, Game1.random.Next(5, 8).ToString());
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        var player = Game1.player;
        player.Increment(DataFields.DaysLeftTranslating, -1);
        if (player.Read<int>(DataFields.DaysLeftTranslating) > 0)
        {
            return;
        }

        player.Write(DataFields.DaysLeftTranslating, null);
        player.Write(DataFields.ReadyToForge, true.ToString());
        this.Disable();
    }
}
