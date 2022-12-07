namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class SveWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SveWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SveWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ModEntry.Config.Arsenal.InfinityPlusOne && Ligo.Integrations.SveConfig is not null;

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Name != "Custom_TreasureCave")
        {
            return;
        }

        e.NewLocation.setTileProperty(10, 7, "Buildings", "Success", $"W {0} 1");
    }
}
