namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop;

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
        Game1.player.WriteIfNotExists(
            DataKeys.DaysLeftTranslating,
            (14 - Game1.player.getFriendshipHeartLevelForNPC("Clint")).ToString());
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        var player = Game1.player;
        player.Increment(DataKeys.DaysLeftTranslating, -1);
        if (Game1.random.NextDouble() < (player.getFriendshipHeartLevelForNPC("Clint") - 6) / 10d)
        {
            player.Increment(DataKeys.DaysLeftTranslating, -1);
        }

        var daysLeft = player.Read<int>(DataKeys.DaysLeftTranslating);
        Log.T($"T - {daysLeft} days left until Clint is done deciphering the blueprint.");
        if (daysLeft <= 0)
        {
            this.Disable();
        }
    }
}
