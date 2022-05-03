namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class ProspectorHuntUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.PlayerState.ProspectorHunt.Update(e.Ticks);
        if (Game1.player.HasProfession(Profession.Prospector, true)) Game1.gameTimeInterval = 0;
    }
}