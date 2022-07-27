namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using TreasureHunts;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntUpdateTickedEvent : UpdateTickedEvent
{
    private ProspectorHunt? Hunt;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ProspectorHuntUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Hunt ??= (ProspectorHunt)ModEntry.State.ProspectorHunt.Value;
        Hunt.Update(e.Ticks);
        if (Game1.player.HasProfession(Profession.Prospector, true)) Game1.gameTimeInterval = 0;
    }
}