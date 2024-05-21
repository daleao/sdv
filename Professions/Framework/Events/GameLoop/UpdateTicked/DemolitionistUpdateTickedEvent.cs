namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Professions.Framework.Buffs;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DemolitionistUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class DemolitionistUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.DemolitionistAdrenaline > 0;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.player.hasBuff("DaLion.Professions.Buffs.DemolitionistExcitedness"))
        {
            return;
        }

        Game1.player.applyBuff(new DemolitionistAdrenalineBuff());
        State.DemolitionistAdrenaline -= State.DemolitionistAdrenaline >= 4 ? 2 : 1;
    }
}
