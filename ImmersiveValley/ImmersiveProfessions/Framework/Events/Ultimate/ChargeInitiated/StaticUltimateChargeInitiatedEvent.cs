namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using JetBrains.Annotations;

using Display;

#endregion using directives

/// <inheritdoc />
[UsedImplicitly]
internal class StaticUltimateChargeInitiatedEvent : UltimateChargeInitiatedEvent
{
    /// <inheritdoc />
    protected override void OnUltimateChargeInitiatedImpl(object sender, UltimateChargeInitiatedEventArgs e)
    {
        EventManager.Enable(typeof(UltimateMeterRenderingHudEvent));
    }
}