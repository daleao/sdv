namespace DaLion.Professions.Framework.Buffs;

#region using directives

using StardewValley.Buffs;

#endregion using directives

internal sealed class DemolitionistExcitednessBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.DemolitionistExcitedness";
    internal const int SHEET_INDEX = 57;

    internal DemolitionistExcitednessBuff()
        : base(
            id: ID,
            source: "Demolitionist",
            displaySource: _I18n.Get("demolitionist.title" + (Game1.player.IsMale ? ".male" : ".female")),
            iconSheetIndex: SHEET_INDEX,
            duration: 17,
            effects: new BuffEffects { Speed = { State.DemolitionistExcitedness }, },
            description: I18n.Demolitionist_Buff_Desc())
    {
    }
}
