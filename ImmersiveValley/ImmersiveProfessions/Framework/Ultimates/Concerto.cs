namespace DaLion.Stardew.Professions.Framework.Ultimates;

#region using directives

using System.Linq;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;
using DaLion.Stardew.Professions.Framework.Sounds;
using DaLion.Stardew.Professions.Framework.VirtualProperties;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Handles Piper ultimate activation.</summary>
public sealed class Concerto : Ultimate
{
    /// <summary>Initializes a new instance of the <see cref="Concerto"/> class.</summary>
    internal Concerto()
        : base("Concerto", 28, Color.LimeGreen, Color.DarkGreen)
    {
    }

    /// <inheritdoc />
    public override string DisplayName =>
        ModEntry.i18n.Get(this.Name.ToLower() + ".title." + (Game1.player.IsMale ? "male" : "female"));

    /// <inheritdoc />
    public override bool CanActivate => base.CanActivate && Game1.player.currentLocation.characters.OfType<Monster>()
        .Any(m => m.IsSlime() && m.IsWithinPlayerThreshold());

    /// <inheritdoc />
    internal override int MillisecondsDuration =>
        (int)(30000 * ((double)this.MaxValue / BaseMaxValue) / ModEntry.Config.SpecialDrainFactor);

    /// <inheritdoc />
    internal override Sfx ActivationSfx => Sfx.PiperConcerto;

    /// <inheritdoc />
    internal override Color GlowColor => Color.LimeGreen;

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();

        foreach (var slime in Game1.player.currentLocation.characters.OfType<GreenSlime>()
                     .Where(c => c.IsWithinPlayerThreshold() && c.Scale < 2f))
        {
            if (Game1.random.NextDouble() <= 0.012 + (Game1.player.team.AverageDailyLuck() / 10.0))
            {
                if (Game1.currentLocation is MineShaft && Game1.player.team.SpecialOrderActive("Wizard2"))
                {
                    slime.makePrismatic();
                }
                else
                {
                    slime.hasSpecialItem.Value = true;
                }
            }

            slime.Set_Piper(Game1.player);
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
                justCreated.setTrajectory(
                    (int)((bigSlimes[i].xVelocity / 8) + Game1.random.Next(-2, 3)),
                    (int)((bigSlimes[i].yVelocity / 8) + Game1.random.Next(-2, 3)));
                justCreated.willDestroyObjectsUnderfoot = false;
                justCreated.moveTowardPlayer(4);
                justCreated.Scale = 0.75f + (Game1.random.Next(-5, 10) / 100f);
                justCreated.currentLocation = Game1.currentLocation;
            }
        }

        Game1.buffsDisplay.removeOtherBuff(this.BuffId);
        Game1.buffsDisplay.addOtherBuff(
            new Buff(
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
                0,
                1,
                this.GetType().Name,
                this.DisplayName)
            {
                which = this.BuffId,
                sheetIndex = 51,
                glow = this.GlowColor,
                millisecondsDuration = this.MillisecondsDuration,
                description = this.Description,
            });

        ModEntry.Events.Enable<SlimeInflationUpdateTickedEvent>();
        this.ActivationSfx.PlayAfterDelay(333);
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();
        ModEntry.Events.Enable<SlimeDeflationUpdateTickedEvent>();
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        this.ChargeValue -= this.MaxValue / 1800d; // lasts 30s * 60 ticks/s -> 1800 ticks
    }
}
