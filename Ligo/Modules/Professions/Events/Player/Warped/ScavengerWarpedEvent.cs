namespace DaLion.Ligo.Modules.Professions.Events.Player;

#region using directives

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
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (ModEntry.State.Professions.ScavengerHunt.Value.IsActive)
        {
            ModEntry.State.Professions.ScavengerHunt.Value.Fail();
        }

        if (e.NewLocation.IsOutdoors && (ModEntry.Config.Professions.AllowScavengerHuntsOnFarm || !e.NewLocation.IsFarm))
        {
            ModEntry.State.Professions.ScavengerHunt.Value.TryStart(e.NewLocation);
        }
    }
}
