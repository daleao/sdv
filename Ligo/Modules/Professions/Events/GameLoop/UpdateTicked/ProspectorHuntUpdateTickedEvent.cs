namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.TreasureHunts;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntUpdateTickedEvent : UpdateTickedEvent
{
    private ProspectorHunt? _hunt;

    /// <summary>Initializes a new instance of the <see cref="ProspectorHuntUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorHuntUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        this._hunt ??= (ProspectorHunt)ModEntry.State.Professions.ProspectorHunt.Value;
        this._hunt.Update(e.Ticks);
        if (Game1.player.HasProfession(Profession.Prospector, true))
        {
            Game1.gameTimeInterval = 0;
        }
    }
}
