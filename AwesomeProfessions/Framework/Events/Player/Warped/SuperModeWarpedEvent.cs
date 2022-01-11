using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Events.Display.RenderingHud;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events.Player.Warped;

internal class SuperModeWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation) || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.IsCombatZone()) ModEntry.EventManager.Enable(typeof(SuperModeGaugeRenderingHudEvent));
        else ModEntry.State.Value.SuperMode.Gauge.CurrentValue = 0.0;
    }
}