namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using StardewModdingAPI.Events;

using Display;
using Extensions;

#endregion using directives

internal class SuperModeWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation) || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.IsCombatZone() && ModEntry.Config.EnableSuperMode)
        {
            EventManager.Enable(typeof(SuperModeGaugeRenderingHudEvent));
            if (ModEntry.PlayerState.Value.SuperMode is {IsActive: true} superMode)
                superMode.Deactivate();
        }
        else
        {
            ModEntry.PlayerState.Value.SuperMode.ChargeValue = 0.0;
            EventManager.Disable(typeof(SuperModeGaugeRenderingHudEvent));
        }
    }
}