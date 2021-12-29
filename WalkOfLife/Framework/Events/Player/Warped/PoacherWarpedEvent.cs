using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class PoacherWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    public override void OnWarped(object sender, WarpedEventArgs e)
    {
        if (e.IsLocalPlayer) ModState.MonstersStolenFrom.Clear();
    }
}