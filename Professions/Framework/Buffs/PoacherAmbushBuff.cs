namespace DaLion.Professions.Framework.Buffs;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.Limits;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buffs;
using StardewValley.Monsters;

#endregion using directives

internal sealed class PoacherAmbushBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.Limit.Ambush";
    internal const int BASE_DURATION = 15_000;
    internal const int SHEET_INDEX = 51;

    internal PoacherAmbushBuff()
        : base(
            id: ID,
            source: "Ambush",
            displaySource: Game1.player.IsMale ? I18n.Poacher_Limit_Title_Male() : I18n.Poacher_Limit_Title_Female(),
            duration: (int)(BASE_DURATION * LimitBreak.GetDurationMultiplier),
            iconSheetIndex: SHEET_INDEX,
            effects: new BuffEffects
            {
                Speed = { -2 },
            },
            description: I18n.Poacher_Limit_Desc_Hidden())
    {
        this.glow = Color.MediumPurple;
    }

    public override void OnAdded()
    {
        SoundBox.PoacherCloak.PlayAll(Game1.player.currentLocation, Game1.player.Tile);
        foreach (var character in Game1.currentLocation.characters)
        {
            if (character is not Monster { IsMonster: true, Player.IsLocalPlayer: true } monster)
            {
                continue;
            }

            monster.focusedOnFarmers = false;
            switch (monster)
            {
                case AngryRoger:
                case Ghost:
                    ModHelper.Reflection.GetField<bool>(monster, "seenPlayer").SetValue(false);
                    break;
                case Bat:
                case RockGolem:
                    ModHelper.Reflection.GetField<NetBool>(monster, "seenPlayer").GetValue().Value = false;
                    break;
                case DustSpirit:
                    ModHelper.Reflection.GetField<bool>(monster, "seenFarmer").SetValue(false);
                    ModHelper.Reflection.GetField<bool>(monster, "chargingFarmer").SetValue(false);
                    break;
                case ShadowGuy:
                case ShadowShaman:
                case Skeleton:
                    ModHelper.Reflection.GetField<bool>(monster, "spottedPlayer").SetValue(false);
                    break;
            }
        }

        EventManager.Enable<AmbushUpdateTickedEvent>();
    }
}
