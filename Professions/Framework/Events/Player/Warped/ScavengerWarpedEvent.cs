namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ScavengerWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ScavengerWarpedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Scavenger, true);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer)
        {
            return;
        }

        State.ScavengerHunt ??= new ScavengerHunt();
        if (State.ScavengerHunt.IsActive)
        {
            State.ScavengerHunt.Fail();
        }

        if (e.NewLocation.currentEvent is not null || !e.NewLocation.IsOutdoors ||
            (e.NewLocation.IsFarm && !Config.AllowScavengerHuntsOnFarm))
        {
            return;
        }

        var chance = Math.Atan(16d / 625d * Data.ReadAs<int>(e.Player, DataKeys.ScavengerHuntStreak));
        if (Game1.random.NextBool(chance))
        {
            e.NewLocation.spawnObjects();
        }
    }
}
