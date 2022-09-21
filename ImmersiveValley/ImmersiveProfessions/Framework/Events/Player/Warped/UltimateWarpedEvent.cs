namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.Events.Display;
using DaLion.Stardew.Professions.Framework.Ultimates;
using DaLion.Stardew.Professions.Framework.VirtualProperties;
using StardewModdingAPI.Events;

#endregion using directives

[UltimateEvent]
[UsedImplicitly]
internal sealed class UltimateWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateWarpedEvent(ProfessionEventManager manager)
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
