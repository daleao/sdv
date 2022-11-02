namespace DaLion.Redux.Framework.Professions.Events.GameLoop;

#region using directives

using DaLion.Redux.Framework.Professions.Ultimates;
using DaLion.Redux.Framework.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UltimateEvent]
[UsedImplicitly]
internal sealed class UltimateGaugeShakeUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateGaugeShakeUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal UltimateGaugeShakeUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Game1.player.Get_Ultimate()!.Hud.UpdateShake();
    }
}
