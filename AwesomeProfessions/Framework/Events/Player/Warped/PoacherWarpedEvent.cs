using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.Player.Warped;

internal class PoacherWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    public override void OnWarped(object sender, WarpedEventArgs e)
    {
        if (e.IsLocalPlayer) ModEntry.State.Value.MonstersStolenFrom.Clear();
    }
}