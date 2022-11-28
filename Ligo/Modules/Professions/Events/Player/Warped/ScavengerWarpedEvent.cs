namespace DaLion.Ligo.Modules.Professions.Events.Player;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ScavengerWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ScavengerWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Scavenger);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.Player.Get_ScavengerHunt().IsActive)
        {
            e.Player.Get_ScavengerHunt().Fail();
        }

        if (e.NewLocation.IsOutdoors && (ModEntry.Config.Professions.AllowScavengerHuntsOnFarm || !e.NewLocation.IsFarm))
        {
            e.Player.Get_ScavengerHunt().TryStart(e.NewLocation);
        }
    }
}
