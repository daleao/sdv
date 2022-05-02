namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using JetBrains.Annotations;
using StardewValley;

using Extensions;
using GameLoop;

#endregion using directives

/// <inheritdoc />
[UsedImplicitly]
internal class StaticUltimateEmptiedEvent : UltimateEmptiedEvent
{
    /// <inheritdoc cref="OnUltimateEmptied" />
    protected override void OnUltimateEmptiedImpl(object sender, UltimateEmptiedEventArgs e)
    {
        EventManager.Disable(typeof(UltimateGaugeShakeUpdateTickedEvent));
        ModEntry.PlayerState.RegisteredUltimate.Meter.ForceStopShake();

        if (ModEntry.PlayerState.RegisteredUltimate.IsActive) ModEntry.PlayerState.RegisteredUltimate.Deactivate();

        if (!Game1.currentLocation.IsDungeon())
            EventManager.Enable(typeof(UltimateGaugeFadeOutUpdateTickedEvent));
    }
}