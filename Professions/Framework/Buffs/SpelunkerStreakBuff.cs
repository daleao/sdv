namespace DaLion.Professions.Framework.Buffs;

internal sealed class SpelunkerStreakBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.SpelunkerStreak";
    private const int SHEET_INDEX = 56;

    internal SpelunkerStreakBuff()
        : base(
            id: ID,
            source: "Spelunker",
            displaySource: _I18n.Get("spelunker.title" + (Game1.player.IsMale ? ".male" : ".female")),
            iconTexture: Game1.buffsIcons,
            iconSheetIndex: SHEET_INDEX,
            duration: 17)
    {
        this.description =
            I18n.Spelunker_Buff_Desc((State.SpelunkerLadderStreak * 0.005f).ToString("P1"));
    }
}
