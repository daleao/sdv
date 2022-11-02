namespace DaLion.Redux.Framework.Professions.Events.GameLoop;

#region using directives

using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Redux.Framework.Professions.TreasureHunts;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerHuntUpdateTickedEvent : UpdateTickedEvent
{
    private ScavengerHunt? _hunt;

    /// <summary>Initializes a new instance of the <see cref="ScavengerHuntUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ScavengerHuntUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        this._hunt ??= (ScavengerHunt)ModEntry.State.Professions.ScavengerHunt.Value;
        this._hunt.Update(e.Ticks);
        if (Game1.player.HasProfession(Profession.Scavenger, true))
        {
            Game1.gameTimeInterval = 0;
        }
    }
}
