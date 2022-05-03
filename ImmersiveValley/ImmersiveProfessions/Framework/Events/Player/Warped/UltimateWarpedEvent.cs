namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Display;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class UltimateWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation) || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.IsDungeon())
        {
            EventManager.Enable(typeof(UltimateMeterRenderingHudEvent));
        }
        else
        {
            ModEntry.PlayerState.RegisteredUltimate.ChargeValue = 0.0;
            EventManager.Disable(typeof(UltimateMeterRenderingHudEvent));
        }
    }
}