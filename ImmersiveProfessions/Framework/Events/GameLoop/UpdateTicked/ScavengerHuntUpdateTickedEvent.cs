namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

using Extensions;

#endregion using directives

internal class ScavengerHuntUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.PlayerState.Value.ScavengerHunt.Update(e.Ticks);
        if (Game1.player.HasProfession(Profession.Scavenger, true)) Game1.gameTimeInterval = 0;
    }
}