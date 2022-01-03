using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class ProspectorWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    public override void OnWarped(object sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;

        ModState.ProspectorHunt ??= new();
        if (ModState.ProspectorHunt.IsActive) ModState.ProspectorHunt.End();
        if (!Game1.eventUp && e.NewLocation is MineShaft)
            ModState.ProspectorHunt.TryStartNewHunt(e.NewLocation);
    }
}