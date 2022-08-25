// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Framework.Ultimates;

#region using directives

using Microsoft.Xna.Framework;
using Sounds;
using System;

#endregion using directives

/// <summary>Handles Brute ultimate activation.</summary>
public sealed class Frenzy : Ultimate
{
    //private double _elapsedSinceDoT;
    public const float PCT_INCREMENT_PER_RAGE_F = 0.01f;

    /// <summary>Construct an instance.</summary>
    internal Frenzy()
    : base(UltimateIndex.BruteFrenzy, Color.OrangeRed, Color.OrangeRed) { }

    #region internal properties

    /// <inheritdoc />
    internal override int BuffId { get; } = (ModEntry.Manifest.UniqueID + (int)UltimateIndex.BruteFrenzy + 4).GetHashCode();

    /// <inheritdoc />
    internal override int MillisecondsDuration =>
        (int)(15000 * ((double)MaxValue / BASE_MAX_VALUE_I) / ModEntry.Config.SpecialDrainFactor);

    /// <inheritdoc />
    internal override SFX ActivationSfx => SFX.BruteRage;

    /// <inheritdoc />
    internal override Color GlowColor => Color.OrangeRed;

    #endregion internal properties

    #region internal methods

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();

        ModEntry.State.BruteKillCounter = 0;

        Game1.buffsDisplay.removeOtherBuff(BuffId);
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                GetType().Name,
                ModEntry.i18n.Get("brute.ulti.name"))
            {
                which = BuffId,
                sheetIndex = 48,
                glow = GlowColor,
                millisecondsDuration = MillisecondsDuration,
                description = ModEntry.i18n.Get("brute.ulti.desc")
            }
        );
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();

        Game1.buffsDisplay.removeOtherBuff(BuffId);

        var who = Game1.player;
        var healed = (int)(who.maxHealth * ModEntry.State.BruteKillCounter * 0.05f);
        who.health = Math.Min(who.health + healed, who.maxHealth);
        who.currentLocation.debris.Add(new(healed,
            new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        ChargeValue -= MaxValue / 900d; // lasts 15s * 60 ticks/s -> 900 ticks
    }

    #endregion internal methods
}