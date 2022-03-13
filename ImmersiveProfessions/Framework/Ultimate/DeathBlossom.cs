namespace DaLion.Stardew.Professions.Framework.Ultimate;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;

using AssetLoaders;

#endregion using directives

/// <summary>Handles Desperado ultimate activation.</summary>
internal sealed class DeathBlossom : Ultimate
{
    /// <summary>Construct an instance.</summary>
    internal DeathBlossom()
    {
        Meter = new(this, Color.DarkGoldenrod);
        Overlay = new(Color.SandyBrown);
        EnableEvents();
    }

    #region public properties

    public static int BuffId { get; } = ModEntry.Manifest.UniqueID.GetHashCode() + (int) UltimateIndex.Desperado + 4;

    public override SFX ActivationSfx => SFX.DesperadoBlossom;
    public override Color GlowColor => Color.DarkGoldenrod;
    public override UltimateIndex Index => UltimateIndex.Desperado;

    #endregion public properties

    #region public methods

    /// <inheritdoc />
    public override void Activate()
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
                millisecondsDuration = (int) (15000 * ((double) MaxValue / BASE_MAX_VALUE_I) * 1.0 / ModEntry.Config.UltimateDrainFactor),
                description = ModEntry.ModHelper.Translation.Get("desperado.ultidesc")
            }
        );
    }

    /// <inheritdoc />
    public override void Countdown(double elapsed)
    {
        ChargeValue -= elapsed * 0.12 / 18.0;
    }

    #endregion public methods
}