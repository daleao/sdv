namespace DaLion.Professions.Framework.Buffs;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

internal sealed class PiperConcertoBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.Limit.Concerto";
    private const int BASE_DURATION = 30_000;
    private const int SHEET_INDEX = 53;

    internal PiperConcertoBuff()
        : base(
            id: ID,
            source: "Piper",
            displaySource: Game1.player.IsMale ? I18n.Piper_Limit_Title_Male() : I18n.Piper_Limit_Title_Female(),
            iconTexture: Game1.buffsIcons,
            iconSheetIndex: SHEET_INDEX,
            duration: (int)(BASE_DURATION * LimitBreak.GetDurationMultiplier),
            description: I18n.Piper_Limit_Desc())
    {
        this.glow = Color.LimeGreen;
    }

    public override void OnAdded()
    {
        SoundBox.PiperProvoke.PlayAll(Game1.player.currentLocation, Game1.player.Tile);
        foreach (var character in Game1.player.currentLocation.characters)
        {
            if (character is not GreenSlime slime || !slime.IsCharacterWithinThreshold() || slime.Scale >= 2f)
            {
                continue;
            }

            slime.Set_Piped(Game1.player, BASE_DURATION);
            slime.focusedOnFarmers = true;
        }

        var bigSlimes = Game1.currentLocation.characters.OfType<BigSlime>().ToList();
        for (var i = bigSlimes.Count - 1; i >= 0; i--)
        {
            var bigSlime = bigSlimes[i];
            bigSlime.Health = 0;
            bigSlime.deathAnimation();
            var toCreate = Game1.random.Next(2, 5);
            while (toCreate-- > 0)
            {
                Game1.currentLocation.characters.Add(new GreenSlime(bigSlime.Position, Game1.CurrentMineLevel));
                var justCreated = Game1.currentLocation.characters[^1];
                justCreated.setTrajectory(
                    (int)((bigSlime.xVelocity / 8) + Game1.random.Next(-2, 3)),
                    (int)((bigSlime.yVelocity / 8) + Game1.random.Next(-2, 3)));
                justCreated.willDestroyObjectsUnderfoot = false;
                justCreated.moveTowardPlayer(4);
                justCreated.Scale = 0.75f + (Game1.random.Next(-5, 10) / 100f);
                justCreated.currentLocation = Game1.currentLocation;
            }
        }

        EventManager.Enable<SlimeInflationUpdateTickedEvent>();
    }
}
