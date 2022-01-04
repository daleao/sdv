using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace TheLion.Stardew.Professions.Framework.Events.Player.Warped;

internal class ProspectorWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    public override void OnWarped(object sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;

        ModEntry.State.Value.ProspectorHunt ??= new();
        if (ModEntry.State.Value.ProspectorHunt.IsActive) ModEntry.State.Value.ProspectorHunt.End();
        if (!Game1.eventUp && e.NewLocation is MineShaft)
            ModEntry.State.Value.ProspectorHunt.TryStartNewHunt(e.NewLocation);
    }
}