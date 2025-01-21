namespace DaLion.Professions.Framework.Buffs;

#region using directives

using DaLion.Professions.Framework.Limits;
using Microsoft.Xna.Framework;

#endregion using directives

internal sealed class DesperadoBlossomBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.Limit.Blossom";
    internal const int BASE_DURATION_MS = 15_000;
    internal const int SHEET_INDEX = 55;

    internal DesperadoBlossomBuff()
        : base(
            id: ID,
            source: "Blossom",
            displaySource: Game1.player.IsMale ? I18n.Desperado_Limit_Title_Male() : I18n.Desperado_Limit_Title_Female(),
            duration: (int)(BASE_DURATION_MS * LimitBreak.DurationMultiplier),
            iconTexture: Game1.buffsIcons,
            iconSheetIndex: SHEET_INDEX,
            description: I18n.Desperado_Limit_Desc())
    {
        this.glow = Color.DarkGoldenrod;
    }

    /// <inheritdoc />
    public override void OnAdded()
    {
        SoundBox.DesperadoWhoosh.PlayAll(Game1.player.currentLocation, Game1.player.Tile);
    }
}
