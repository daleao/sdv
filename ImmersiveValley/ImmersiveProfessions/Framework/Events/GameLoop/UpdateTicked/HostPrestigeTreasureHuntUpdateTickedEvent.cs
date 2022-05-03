namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal class HostPrestigeTreasureHuntUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        Game1.gameTimeInterval = 0;
    }
}