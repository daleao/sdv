namespace DaLion.Redux.Professions.Events.Player;

#region using directives

using DaLion.Redux.Professions.Events.Display;
using DaLion.Redux.Professions.Extensions;
using DaLion.Redux.Professions.Ultimates;
using DaLion.Redux.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UltimateEvent]
[UsedImplicitly]
internal sealed class UltimateWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal UltimateWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.GetType() == e.OldLocation.GetType())
        {
            return;
        }

        if (e.NewLocation.IsDungeon())
        {
            this.Manager.Enable<UltimateMeterRenderingHudEvent>();
        }
        else
        {
            e.Player.Get_Ultimate()!.ChargeValue = 0.0;
            this.Manager.Disable<UltimateMeterRenderingHudEvent>();
        }
    }
}
