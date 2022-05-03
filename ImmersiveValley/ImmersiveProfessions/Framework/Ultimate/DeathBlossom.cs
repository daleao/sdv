namespace DaLion.Stardew.Professions.Framework.Ultimate;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;

using Sounds;

#endregion using directives

/// <summary>Handles Desperado ultimate activation.</summary>
internal sealed class DeathBlossom : Ultimate
{
    /// <summary>Construct an instance.</summary>
    internal DeathBlossom()
    : base(Color.DarkGoldenrod, Color.SandyBrown)
    {
    }

    #region public properties

    /// <summary>The ID of the buff that displays while Death Blossom is active.</summary>
    public static int BuffId { get; } = ModEntry.Manifest.UniqueID.GetHashCode() + (int) UltimateIndex.Blossom + 4;

    /// <inheritdoc />
    public override UltimateIndex Index => UltimateIndex.Blossom;

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
                ModEntry.ModHelper.Translation.Get("desperado.ulti"))
            {
                which = BuffId,
                sheetIndex = 51,
                glow = GlowColor,
                millisecondsDuration = (int) (15000 * ((double) MaxValue / BASE_MAX_VALUE_I) / ModEntry.Config.UltimateDrainFactor),
                description = ModEntry.ModHelper.Translation.Get("desperado.ultidesc")
            }
        );
    }

    /// <inheritdoc />
    internal override void Countdown(double elapsed)
    {
        ChargeValue -= elapsed * 0.12 / 18.0;
    }

    #endregion internal methods
}