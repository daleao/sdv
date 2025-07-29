namespace DaLion.Professions.Framework.Events.GameLoop.ReturnedToTitle;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="TreasureHunterReturnedToTitleEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class TreasureHunterReturnedToTitleEvent(EventManager? manager = null)
    : ReturnedToTitleEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.ScavengerHunt is not null || State.ProspectorHunt is not null;

    /// <inheritdoc />
    protected override void OnReturnedToTitleImpl(object? sender, ReturnedToTitleEventArgs e)
    {
        State.ScavengerHunt?.FlushMapCache();
        Log.D("Flushed treasure hunt caches.");
    }
}
