namespace DaLion.Overhaul.Modules.Professions.Ultimates;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
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
        _I18n.Get(this.Name.ToLower() + ".title." + (Game1.player.IsMale ? "male" : "female"));

    /// <inheritdoc />
    public override bool CanActivate => base.CanActivate && Game1.player.currentLocation.characters.OfType<Monster>()
        .Any(m => m.IsSlime() && m.IsWithinPlayerThreshold());

    /// <inheritdoc />
    public override IProfession Profession => Professions.Profession.Piper;

    /// <inheritdoc />
    internal override int MillisecondsDuration =>
        (int)(30000 * ((double)this.MaxValue / BaseMaxValue) / ProfessionsModule.Config.LimitDrainFactor);

    /// <inheritdoc />
    internal override Sfx ActivationSfx => Sfx.PiperConcerto;

    /// <inheritdoc />
    internal override Color GlowColor => Color.LimeGreen;

    /// <summary>Gets or sets the number of ticks since the latest contact with a Slime.</summary>
    internal int SlimeContactTimer { get; set; }

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();
        for (var i = 0; i < Game1.player.currentLocation.characters.Count; i++)
        {
            var character = Game1.player.currentLocation.characters[i];
            if (character is not GreenSlime slime || !slime.IsWithinPlayerThreshold() || slime.Scale >= 2f)
            {
                continue;
            }

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

            slime.Set_Piped(Game1.player);
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
                sheetIndex = this.BuffSheetIndex,
                glow = this.GlowColor,
                millisecondsDuration = this.MillisecondsDuration,
                description = this.Description,
            });

        EventManager.Enable<SlimeInflationUpdateTickedEvent>();
        this.ActivationSfx.PlayAfterDelay(333);
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();
        EventManager.Enable<SlimeDeflationUpdateTickedEvent>();
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        this.ChargeValue -= this.MaxValue / 1800d; // lasts 30s * 60 ticks/s -> 1800 ticks
    }
}
