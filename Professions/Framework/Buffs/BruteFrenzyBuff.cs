namespace DaLion.Professions.Framework.Buffs;

#region using directives

using DaLion.Core.Framework.Extensions;
using DaLion.Professions.Framework.Limits;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

internal sealed class BruteFrenzyBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.Limits.Frenzy";
    internal const int BASE_DURATION = 15_000;
    internal const int SHEET_INDEX = 49;

    internal BruteFrenzyBuff()
        : base(
            id: ID,
            source: "Frenzy",
            displaySource: Game1.player.IsMale ? I18n.Brute_Limit_Title_Male() : I18n.Brute_Limit_Title_Female(),
            duration: (int)(BASE_DURATION * LimitBreak.GetDurationMultiplier),
            iconSheetIndex: SHEET_INDEX,
            description: I18n.Brute_Limit_Desc())
    {
        this.glow = Color.OrangeRed;
    }

    public override void OnAdded()
    {
        SoundBox.BruteRage.PlayAll(Game1.player.currentLocation, Game1.player.Tile);
        foreach (var character in Game1.currentLocation.characters)
        {
            if (character is not Monster { IsMonster: true, Player.IsLocalPlayer: true, } monster)
            {
                continue;
            }

            monster.Fear(2000);
        }
    }
}
