using StardewModdingAPI.Events;
using DaLion.Stardew.Professions.Framework.Events.Display;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework.Events.Player;

internal class SuperModeWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation) || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.IsCombatZone() && ModEntry.Config.EnableSuperMode) ModEntry.EventManager.Enable(typeof(SuperModeGaugeRenderingHudEvent));
        else ModEntry.State.Value.SuperMode.Gauge.CurrentValue = 0.0;
    }
}