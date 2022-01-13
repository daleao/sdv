using StardewModdingAPI.Events;

namespace DaLion.Stardew.Professions.Framework.Events.Player;

internal class PoacherWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (!e.NewLocation.Equals(e.OldLocation)) ModEntry.State.Value.MonstersStolenFrom.Clear();
    }
}