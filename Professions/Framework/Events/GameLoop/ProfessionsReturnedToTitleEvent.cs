namespace DaLion.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProfessionsReturnedToTitleEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionsReturnedToTitleEvent(EventManager? manager = null)
    : ReturnedToTitleEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnReturnedToTitleImpl(object? sender, ReturnedToTitleEventArgs e)
    {
        this.Manager.Reset();
        PerScreenState.ResetAllScreens();
        Pathfinder = null;
        PathfinderAsync?.Dispose();
        PathfinderAsync = null;
    }
}
