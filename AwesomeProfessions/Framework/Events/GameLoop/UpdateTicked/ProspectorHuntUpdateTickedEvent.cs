namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

using Extensions;

#endregion using directives

internal class ProspectorHuntUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.PlayerState.Value.ProspectorHunt.Update(e.Ticks);
        if (Game1.player.HasProfession(Profession.Prospector, true)) Game1.gameTimeInterval = 0;
    }
}