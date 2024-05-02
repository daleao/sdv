namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SveWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SveWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SveWarpedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Name.Contains("Galdora") || e.OldLocation.Name.Contains("Galdora"))
        {
            ModHelper.GameContent.InvalidateCache($"{UniqueId}/LimitGauge");
        }
    }
}
