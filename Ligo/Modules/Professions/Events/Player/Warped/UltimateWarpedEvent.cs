namespace DaLion.Ligo.Modules.Professions.Events.Player;

#region using directives

using DaLion.Ligo.Modules.Professions.Events.Display;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
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
