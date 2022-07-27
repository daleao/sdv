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

    #region public properties

    /// <summary>The ID of the buff that displays while Death Blossom is active.</summary>
    public static int BuffId { get; } = (ModEntry.Manifest.UniqueID + (int)UltimateIndex.DesperadoBlossom + 4).GetHashCode();

    #endregion public properties

    #region internal properties

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
                ModEntry.i18n.Get("desperado.ulti"))
            {
                which = BuffId,
                sheetIndex = 51,
                glow = GlowColor,
                millisecondsDuration = (int)(15000 * ((double)MaxValue / BASE_MAX_VALUE_I) / ModEntry.Config.SpecialDrainFactor),
                description = ModEntry.i18n.Get("desperado.ultidesc")
            }
        );
    }

    /// <inheritdoc />
    internal override void Countdown(double elapsed)
    {
        ChargeValue -= elapsed * 0.02 / 3.0; // lasts 15s
    }

    #endregion internal methods
}