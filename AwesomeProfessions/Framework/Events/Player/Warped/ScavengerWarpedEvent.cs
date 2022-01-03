using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class ScavengerWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    public override void OnWarped(object sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;

        ModState.ScavengerHunt ??= new();
        if (ModState.ScavengerHunt.IsActive) ModState.ScavengerHunt.End();
        if (!Game1.eventUp && e.NewLocation.IsOutdoors &&
            (ModEntry.Config.AllowScavengerHuntsOnFarm || !e.NewLocation.IsFarm))
            ModState.ScavengerHunt.TryStartNewHunt(e.NewLocation);
    }
}