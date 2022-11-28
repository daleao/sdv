namespace DaLion.Ligo.Modules.Professions.Events.Player;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProspectorWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Prospector);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.Player.Get_ProspectorHunt().IsActive)
        {
            e.Player.Get_ProspectorHunt().Fail();
        }

        if (e.NewLocation is MineShaft shaft && !shaft.IsTreasureOrSafeRoom())
        {
            e.Player.Get_ProspectorHunt().TryStart(e.NewLocation);
        }
    }
}
