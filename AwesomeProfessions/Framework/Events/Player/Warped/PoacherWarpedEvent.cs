using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.Player.Warped;

internal class PoacherWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (!e.NewLocation.Equals(e.OldLocation)) ModEntry.State.Value.MonstersStolenFrom.Clear();
    }
}