namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Professions.Framework.Events.Display.RenderingHud;
using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LimitWarpedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[LimitEvent]
[UsedImplicitly]
internal sealed class LimitWarpedEvent(EventManager? manager = null)
    : WarpedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Config.Masteries.UnlockLimitBreaks && State.LimitBreak is not null;

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer || e.NewLocation.GetType() == e.OldLocation.GetType())
        {
            return;
        }

        if (e.NewLocation.IsEnemyArea())
        {
            this.Manager.Enable<LimitGaugeRenderingHudEvent>();
        }
        else
        {
            State.LimitBreak!.ChargeValue = 0d;
            this.Manager.Disable<LimitGaugeRenderingHudEvent>();
        }
    }
}
