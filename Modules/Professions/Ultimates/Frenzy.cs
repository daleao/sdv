namespace DaLion.Overhaul.Modules.Professions.Ultimates;

#region using directives

using DaLion.Overhaul.Modules.Core.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Handles Brute ultimate activation.</summary>
public sealed class Frenzy : Ultimate
{
    /// <summary>Initializes a new instance of the <see cref="Frenzy"/> class.</summary>
    internal Frenzy()
        : base("Frenzy", Profession.Brute, Color.OrangeRed, Color.OrangeRed)
    {
    }

    /// <inheritdoc />
    public override string DisplayName { get; } = I18n.Frenzy_Title();

    /// <inheritdoc />
    public override string Description { get; } = I18n.Frenzy_Desc();

    /// <inheritdoc />
    internal override int MillisecondsDuration =>
        (int)(15000 * ((double)this.MaxValue / BaseMaxValue) / ProfessionsModule.Config.LimitDrainFactor);

    /// <inheritdoc />
    internal override SoundEffectPlayer ActivationSfx => SoundEffectPlayer.BruteRage;

    /// <inheritdoc />
    internal override Color GlowColor => Color.OrangeRed;

    /// <summary>Gets or sets the number of enemies defeated while active.</summary>
    internal int KillCount { get; set; }

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();
        this.KillCount = 0;

        for (var i = 0; i < Game1.currentLocation.characters.Count; i++)
        {
            if (Game1.currentLocation.characters[i] is not Monster { IsMonster: true, Player.IsLocalPlayer: true } monster)
            {
                continue;
            }

            monster.Fear(2000);
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
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();

        Game1.buffsDisplay.removeOtherBuff(this.BuffId);

        var who = Game1.player;
        var healed = (int)(who.maxHealth * this.KillCount * 0.05f);
        who.health = Math.Min(who.health + healed, who.maxHealth);
        who.currentLocation.debris.Add(new Debris(
            healed,
            new Vector2(who.getStandingX() + 8, who.getStandingY()),
            Color.Lime,
            1f,
            who));
        Game1.playSound("healSound");
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        this.ChargeValue -= this.MaxValue / 900d; // lasts 15s * 60 ticks/s -> 900 ticks
    }
}
