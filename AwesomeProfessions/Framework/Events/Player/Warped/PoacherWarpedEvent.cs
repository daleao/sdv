namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal class PoacherWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (!e.NewLocation.Equals(e.OldLocation)) ModEntry.State.Value.MonstersStolenFrom.Clear();
    }
}