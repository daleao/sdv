namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using JetBrains.Annotations;

using GameLoop;
using Input;

#endregion using directives

/// <inheritdoc />
[UsedImplicitly]
internal class StaticUltimateFullyChargedEvent : UltimateFullyChargedEvent
{
    /// <inheritdoc cref="OnUltimateFullyCharged" />
    protected override void OnUltimateFullyChargedImpl(object sender, UltimateFullyChargedEventArgs e)
    {
        EventManager.Enable(typeof(UltimateButtonsChangedEvent), typeof(UltimateGaugeShakeUpdateTickedEvent));
    }
}