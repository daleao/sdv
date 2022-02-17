namespace DaLion.Stardew.Professions.Framework.SuperMode;

#region using directives

using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

using AssetLoaders;
using Events.GameLoop;
using Extensions;

#endregion using directives

/// <summary>Handles Piper Eubstance activation.</summary>
internal sealed class PiperEubstance : SuperMode
{
    private static int _InflationCost => (int) (50 * 3 / ModEntry.Config.SuperModeDrainFactor);

    /// <summary>Construct an instance.</summary>
    internal PiperEubstance()
    {
        Gauge = new(Color.LimeGreen);
        Overlay = new(Color.DarkGreen);
        EnableEvents();
    }

    #region public properties

    public override SFX ActivationSfx => SFX.PiperFluidity;
    public override Color GlowColor => Color.LimeGreen;
    public override SuperModeIndex Index => SuperModeIndex.Piper;

    #endregion public properties

    #region public methods

    /// <inheritdoc />
    public override void Activate()
    {
        if (Gauge.CurrentValue < _InflationCost)
        {
            Game1.playSound("cancel");
            return;
        }

        SoundBank.Play(ActivationSfx);
        foreach (var slime in Game1.player.currentLocation.characters.OfType<GreenSlime>()
                     .Where(s => s.IsWithinPlayerThreshold()))
        {
            if (slime.Scale >= 2f) continue;

            if (Gauge.CurrentValue < _InflationCost) break;

            Gauge.CurrentValue -= _InflationCost;

            if (Game1.random.NextDouble() <= 0.012 + Game1.player.team.AverageDailyLuck() / 10.0)
            {
                if (Game1.currentLocation is MineShaft && Game1.player.team.SpecialOrderActive("Wizard2"))
                    slime.makePrismatic();
                else slime.hasSpecialItem.Value = true;
            }

            ModEntry.State.Value.SuperfluidSlimes.Add(new(slime, Game1.player));
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

        EventManager.Enable(typeof(PiperEubstanceActiveUpdateTickedEvent), typeof(SlimeInflationUpdateTickedEvent),
            typeof(SlimeDeflationUpdateTickedEvent));
    }

    /// <inheritdoc />
    public override void Deactivate()
    {
    }

    /// <inheritdoc />
    public override void AddBuff()
    {
        if (Gauge.CurrentValue < 10.0) return;

        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int)SuperModeIndex.Piper;
        var magnitude = Game1.player.GetPiperSlimeAttackSpeed().ToString("0.0");
        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == buffId);
        if (buff == null)
            Game1.buffsDisplay.addOtherBuff(
                new(0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    1,
                    GetType().Name,
                    ModEntry.ModHelper.Translation.Get("piper.buff"))
                {
                    which = buffId,
                    sheetIndex = 38,
                    millisecondsDuration = 0,
                    description = ModEntry.ModHelper.Translation.Get("piper.buffdesc", new { magnitude })
                });
    }

    #endregion public methods

    #region protected methods

    /// <inheritdoc />
    protected override bool CanActivate()
    {
        return !Gauge.IsEmpty && Game1.player.currentLocation.characters.OfType<Monster>()
            .Any(m => m.IsSlime() && m.IsWithinPlayerThreshold());
    }

    #endregion protected methods
}