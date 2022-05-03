namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Framework.TreasureHunt;

#endregion using directives

[UsedImplicitly]
internal class ScavengerWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        ModEntry.PlayerState.ScavengerHunt ??= new ScavengerHunt();
        if (ModEntry.PlayerState.ScavengerHunt.IsActive) ModEntry.PlayerState.ScavengerHunt.Fail();
        if (!Game1.eventUp && e.NewLocation.IsOutdoors &&
            (ModEntry.Config.AllowScavengerHuntsOnFarm || !e.NewLocation.IsFarm))
            ModEntry.PlayerState.ScavengerHunt.TryStart(e.NewLocation);
    }
}