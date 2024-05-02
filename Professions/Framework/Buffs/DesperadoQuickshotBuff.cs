namespace DaLion.Professions.Framework.Buffs;

internal sealed class DesperadoQuickshotBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.DesperadoQuickshot";
    internal const int SHEET_INDEX = 54;

    internal DesperadoQuickshotBuff()
        : base(
            id: ID,
            source: "Desperado",
            displaySource: _I18n.Get("desperado.title" + (Game1.player.IsMale ? ".male" : ".female")) + " " +
                           I18n.Desperado_Buff_Name(),
            iconSheetIndex: SHEET_INDEX,
            duration: 17,
            description: I18n.Desperado_Buff_Desc())
    {
    }
}
