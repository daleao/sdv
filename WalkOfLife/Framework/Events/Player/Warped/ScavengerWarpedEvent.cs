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
        if (Game1.CurrentEvent is null && e.NewLocation.IsOutdoors &&
            !(e.NewLocation.IsFarm || e.NewLocation.NameOrUniqueName == "Town"))
            ModState.ScavengerHunt.TryStartNewHunt(e.NewLocation);
    }
}