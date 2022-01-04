using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events.Player.Warped;

internal class ScavengerWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    public override void OnWarped(object sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;

        ModEntry.State.Value.ScavengerHunt ??= new();
        if (ModEntry.State.Value.ScavengerHunt.IsActive) ModEntry.State.Value.ScavengerHunt.End();
        if (!Game1.eventUp && e.NewLocation.IsOutdoors &&
            (ModEntry.Config.AllowScavengerHuntsOnFarm || !e.NewLocation.IsFarm))
            ModEntry.State.Value.ScavengerHunt.TryStartNewHunt(e.NewLocation);
    }
}