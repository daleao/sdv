namespace DaLion.Overhaul.Modules.Professions.Ultimates;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Handles Desperado ultimate activation.</summary>
public sealed class DeathBlossom : Ultimate
{
    /// <summary>Initializes a new instance of the <see cref="DeathBlossom"/> class.</summary>
    internal DeathBlossom()
        : base("Blossom", Profession.Desperado, Color.DarkGoldenrod, Color.SandyBrown)
    {
    }

    /// <inheritdoc />
    public override string DisplayName { get; } = I18n.Blossom_Title();

    /// <inheritdoc />
    public override string Description { get; } = I18n.Blossom_Desc();

    /// <inheritdoc />
    internal override int MillisecondsDuration =>
        (int)(15000 * ((double)this.MaxValue / BaseMaxValue) / ProfessionsModule.Config.Limit.LimitDrainFactor);

    /// <inheritdoc />
    internal override SoundEffectPlayer ActivationSfx => SoundEffectPlayer.DesperadoBlossom;

    /// <inheritdoc />
    internal override Color GlowColor => Color.DarkGoldenrod;

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();
        SoundEffectPlayer.GunCock.PlayAfterDelay(100);
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
    internal override void Countdown()
    {
        this.ChargeValue -= this.MaxValue / 900d; // lasts 15s * 60 ticks/s -> 900 ticks
    }
}
