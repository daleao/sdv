namespace DaLion.Stardew.Professions.Framework.Ultimate;

#region using directives

using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

using Extensions;
using Events.GameLoop;
using Framework.Events.Ultimate;
using Sounds;

#endregion using directives

/// <summary>Handles Piper ultimate activation.</summary>
internal sealed class Pandemonia : Ultimate
{
    private static int _InflationCost => (int) (35 * ModEntry.Config.UltimateDrainFactor);

    /// <summary>Construct an instance.</summary>
    internal Pandemonia()
    {
        Meter = new(this, Color.LimeGreen);
        Overlay = new(Color.DarkGreen);
        
        EventManager.Enable(typeof(PandemoniaUltimateEmptiedEvent));
    }

    #region public properties

    public override SFX ActivationSfx => SFX.PiperFluidity;
    public override Color GlowColor => Color.LimeGreen;
    public override UltimateIndex Index => UltimateIndex.Piper;

    #endregion public properties

    #region public methods

    /// <inheritdoc />
    public override void Activate()
    {
        if (ChargeValue < _InflationCost)
        {
            Game1.playSound("cancel");
            return;
        }

        EventManager.Disable(typeof(UltimateGaugeShakeUpdateTickedEvent));
        SoundBank.Play(ActivationSfx);
        foreach (var slime in Game1.player.currentLocation.characters.OfType<GreenSlime>()
                     .Where(s => s.IsWithinPlayerThreshold() && s.Scale < 2f && !s.ReadDataAs<bool>("Piped")))
        {
            if (ChargeValue < _InflationCost) break;
            ChargeValue -= _InflationCost;

            if (Game1.random.NextDouble() <= 0.012 + Game1.player.team.AverageDailyLuck() / 10.0)
            {
                if (Game1.currentLocation is MineShaft && Game1.player.team.SpecialOrderActive("Wizard2"))
                    slime.makePrismatic();
                else slime.hasSpecialItem.Value = true;
            }

            slime.MakePipedSlime(Game1.player);
            ModEntry.PlayerState.PipedSlimes.Add(slime);
        }

        var bigSlimes = Game1.currentLocation.characters.OfType<BigSlime>().ToList();
        for (var i = bigSlimes.Count - 1; i >= 0; --i)
        {
            bigSlimes[i].Health = 0;
            bigSlimes[i].deathAnimation();
            var toCreate = Game1.random.Next(2, 5);
            while (toCreate-- > 0)
            {
                Game1.currentLocation.characters.Add(new GreenSlime(bigSlimes[i].Position, Game1.CurrentMineLevel));
                var justCreated = Game1.currentLocation.characters[^1];
                justCreated.setTrajectory((int) (bigSlimes[i].xVelocity / 8 + Game1.random.Next(-2, 3)),
                    (int) (bigSlimes[i].yVelocity / 8 + Game1.random.Next(-2, 3)));
                justCreated.willDestroyObjectsUnderfoot = false;
                justCreated.moveTowardPlayer(4);
                justCreated.Scale = 0.75f + Game1.random.Next(-5, 10) / 100f;
                justCreated.currentLocation = Game1.currentLocation;
            }
        }

        EventManager.Enable(typeof(SlimeInflationUpdateTickedEvent), typeof(SlimeDeflationUpdateTickedEvent),
            typeof(UltimateCountdownUpdateTickedEvent));
    }

    /// <inheritdoc />
    public override void Deactivate()
    {
    }

    /// <inheritdoc />
    public override void Countdown(double elapsed)
    {
        var piped = ModEntry.PlayerState.PipedSlimes;
        if (!piped.Any())
        {
            EventManager.Disable(typeof(UltimateCountdownUpdateTickedEvent));
            return;
        }

        foreach (var slime in piped) slime.Countdown(elapsed);
    }

    #endregion public methods

    #region protected methods

    /// <inheritdoc />
    protected override bool CanActivate()
    {
        return !IsEmpty && Game1.player.currentLocation.characters.OfType<Monster>()
            .Any(m => m.IsSlime() && m.IsWithinPlayerThreshold());
    }

    #endregion protected methods
}