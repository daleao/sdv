namespace DaLion.Professions.Framework.Buffs;

internal sealed class ProspectorHuntBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.ProspectorHunt";
    internal const int SHEET_INDEX = 1851;

    internal ProspectorHuntBuff()
        : base(
            id: ID,
            source: "Prospector Hunt",
            displaySource: _I18n.Get("prospector.title" + (Game1.player.IsMale ? ".male" : ".female")),
            duration: Config.ProspectorHuntTimeLimit * 1000,
            iconTexture: Game1.mouseCursors,
            iconSheetIndex: SHEET_INDEX,
            description: I18n.Scavenger_HuntStarted())
    {
    }
}
