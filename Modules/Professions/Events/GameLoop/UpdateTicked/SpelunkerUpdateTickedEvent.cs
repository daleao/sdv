namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class SpelunkerUpdateTickedEvent : UpdateTickedEvent
{
    private readonly int _buffId = (Manifest.UniqueID + VanillaProfession.Spelunker).GetHashCode();

    /// <summary>Initializes a new instance of the <see cref="SpelunkerUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SpelunkerUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.currentLocation is not MineShaft)
        {
            return;
        }

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == this._buffId);
        if (buff is not null)
        {
            return;
        }

        var ladderChance = (ProfessionsModule.State.SpelunkerLadderStreak * 0.005f).ToString("P1");
        var speed = Math.Min(
            (ProfessionsModule.State.SpelunkerLadderStreak / 10) + 1,
            (int)ProfessionsModule.Config.SpelunkerSpeedCeiling);
        Game1.buffsDisplay.addOtherBuff(
            new Buff(
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                speed,
                0,
                0,
                1,
                "Spelunker",
                _I18n.Get("spelunker.title" + (Game1.player.IsMale ? ".male" : ".female")))
            {
                which = this._buffId,
                sheetIndex = VanillaProfession.SpelunkerStreakSheetIndex,
                millisecondsDuration = 0,
                description =
                    I18n.Spelunker_Buff_Desc(ladderChance, speed),
            });
    }
}
