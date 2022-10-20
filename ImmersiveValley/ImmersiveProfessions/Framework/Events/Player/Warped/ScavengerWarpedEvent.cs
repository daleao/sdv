namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using DaLion.Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ScavengerWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ScavengerWarpedEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (ModEntry.State.ScavengerHunt.Value.IsActive)
        {
            ModEntry.State.ScavengerHunt.Value.Fail();
        }

        if (e.NewLocation.IsOutdoors && (ModEntry.Config.AllowScavengerHuntsOnFarm || !e.NewLocation.IsFarm))
        {
            ModEntry.State.ScavengerHunt.Value.TryStart(e.NewLocation);
        }
    }
}
