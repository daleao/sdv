namespace DaLion.Professions.Framework.Buffs;

internal sealed class ScavengerHuntBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.ScavengerHunt";
    internal const int SHEET_INDEX = 1809;

    internal ScavengerHuntBuff()
        : base(
            id: ID,
            source: "Scavenger Hunt",
            displaySource: _I18n.Get("scavenger.title" + (Game1.player.IsMale ? ".male" : ".female")),
            duration: Config.ScavengerHuntTimeLimit * 1000,
            iconTexture: Game1.mouseCursors,
            iconSheetIndex: SHEET_INDEX,
            description: I18n.Prospector_HuntStarted())
    {
    }
}
