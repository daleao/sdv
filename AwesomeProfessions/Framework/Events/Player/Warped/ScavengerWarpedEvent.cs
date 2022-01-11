using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.TreasureHunt;

namespace TheLion.Stardew.Professions.Framework.Events.Player.Warped;

internal class ScavengerWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        ModEntry.State.Value.ScavengerHunt ??= new ScavengerHunt();
        if (ModEntry.State.Value.ScavengerHunt.IsActive) ModEntry.State.Value.ScavengerHunt.End();
        if (!Game1.eventUp && e.NewLocation.IsOutdoors &&
            (ModEntry.Config.AllowScavengerHuntsOnFarm || !e.NewLocation.IsFarm))
            ModEntry.State.Value.ScavengerHunt.TryStartNewHunt(e.NewLocation);
    }
}