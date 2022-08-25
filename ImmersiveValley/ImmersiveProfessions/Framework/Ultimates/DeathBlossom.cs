namespace DaLion.Stardew.Professions.Framework.Ultimates;

#region using directives

using Microsoft.Xna.Framework;
using Sounds;

#endregion using directives

/// <summary>Handles Desperado ultimate activation.</summary>
public sealed class DeathBlossom : Ultimate
{
    /// <summary>Construct an instance.</summary>
    internal DeathBlossom()
    : base(UltimateIndex.DesperadoBlossom, Color.DarkGoldenrod, Color.SandyBrown) { }

    #region internal properties

    /// <inheritdoc />
    internal override int BuffId { get; } = (ModEntry.Manifest.UniqueID + (int)UltimateIndex.DesperadoBlossom + 4).GetHashCode();

    /// <inheritdoc />
    internal override int MillisecondsDuration =>
        (int)(15000 * ((double)MaxValue / BASE_MAX_VALUE_I) / ModEntry.Config.SpecialDrainFactor);

    /// <inheritdoc />
    internal override SFX ActivationSfx => SFX.DesperadoBlossom;

    /// <inheritdoc />
    internal override Color GlowColor => Color.DarkGoldenrod;

    #endregion internal properties

    #region internal methods

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();

        Game1.buffsDisplay.removeOtherBuff(BuffId);
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                GetType().Name,
                ModEntry.i18n.Get("desperado.ulti.name"))
            {
                which = BuffId,
                sheetIndex = 51,
                glow = GlowColor,
                millisecondsDuration = MillisecondsDuration,
                description = ModEntry.i18n.Get("desperado.ulti.desc")
            }
        );
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        ChargeValue -= MaxValue / 900d; // lasts 15s * 60 ticks/s -> 900 ticks
    }

    #endregion internal methods
}