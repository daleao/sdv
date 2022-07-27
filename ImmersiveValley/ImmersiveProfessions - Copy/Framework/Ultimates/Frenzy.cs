// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Framework.Ultimates;

#region using directives

using Extensions;
using Microsoft.Xna.Framework;
using Sounds;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Linq;

#endregion using directives

/// <summary>Handles Brute ultimate activation.</summary>
public sealed class Frenzy : Ultimate
{
    //private double _elapsedSinceDoT;
    public const float PCT_INCREMENT_PER_RAGE_F = 0.01f;

    /// <summary>Construct an instance.</summary>
    internal Frenzy()
    : base(UltimateIndex.BruteFrenzy, Color.OrangeRed, Color.OrangeRed) { }

    #region public properties

    /// <summary>The ID of the buff that displays while Frenzy is active.</summary>
    public static int BuffId { get; } = (ModEntry.Manifest.UniqueID + (int)UltimateIndex.BruteFrenzy + 4).GetHashCode();

    #endregion public properties

    #region internal properties

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

        ModEntry.Player.BruteKillCounter = 0;

        Game1.buffsDisplay.removeOtherBuff(BuffId);
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                GetType().Name,
                ModEntry.i18n.Get("brute.ulti"))
            {
                which = BuffId,
                sheetIndex = 48,
                glow = GlowColor,
                millisecondsDuration = (int)(15000 * ((double)MaxValue / BASE_MAX_VALUE_I) / ModEntry.Config.SpecialDrainFactor),
                description = ModEntry.i18n.Get("brute.ultidesc")
            }
        );
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();

        Game1.buffsDisplay.removeOtherBuff(BuffId);

        var who = Game1.player;
        var healed = (int)(who.maxHealth * ModEntry.Player.BruteKillCounter * 0.05f);
        who.health = Math.Min(who.health + healed, who.maxHealth);
        who.currentLocation.debris.Add(new(healed,
            new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));
    }

    /// <inheritdoc />
    internal override void Countdown(double elapsed)
    {
        ChargeValue -= elapsed * 0.02 / 3.0; // lasts 15s
    }

    #endregion internal methods
}