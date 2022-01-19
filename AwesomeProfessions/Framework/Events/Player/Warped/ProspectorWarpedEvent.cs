namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

using Extensions;
using TreasureHunt;

#endregion using directives

internal class ProspectorWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        ModEntry.State.Value.ProspectorHunt ??= new ProspectorHunt();
        if (ModEntry.State.Value.ProspectorHunt.IsActive) ModEntry.State.Value.ProspectorHunt.Fail();
        if (!Game1.eventUp && e.NewLocation is MineShaft shaft && !shaft.IsTreasureOrSafeRoom())
            ModEntry.State.Value.ProspectorHunt.TryStartNewHunt(e.NewLocation);
    }
}