namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using Extensions;
using StardewModdingAPI.Events;
using TreasureHunts;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerHuntUpdateTickedEvent : UpdateTickedEvent
{
    private ScavengerHunt? Hunt;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ScavengerHuntUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Hunt ??= (ScavengerHunt)ModEntry.State.ScavengerHunt.Value;
        Hunt.Update(e.Ticks);
        if (Game1.player.HasProfession(Profession.Scavenger, true)) Game1.gameTimeInterval = 0;
    }
}