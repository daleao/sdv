// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Framework.Ultimates;

#region using directives

using Events.GameLoop;
using Microsoft.Xna.Framework;
using Netcode;
using Sounds;
using StardewValley.Monsters;
using System.Linq;

#endregion using directives

/// <summary>Handles Poacher ultimate activation.</summary>
public sealed class Ambush : Ultimate
{
    /// <summary>Construct an instance.</summary>
    internal Ambush()
    : base(UltimateIndex.PoacherAmbush, Color.MediumPurple, Color.MidnightBlue) { }

    #region internal properties

    /// <inheritdoc />
    internal override int BuffId { get; } = (ModEntry.Manifest.UniqueID + (int)UltimateIndex.PoacherAmbush + 4).GetHashCode();

    /// <inheritdoc />
    internal override int MillisecondsDuration =>
        (int)(15000 * ((double)MaxValue / BASE_MAX_VALUE_I) / ModEntry.Config.SpecialDrainFactor);

    /// <inheritdoc />
    internal override SFX ActivationSfx => SFX.PoacherAmbush;

    /// <inheritdoc />
    internal override Color GlowColor => Color.MediumPurple;

    /// <summary>Whether the double crit. power buff is active.</summary>
    internal bool IsGrantingCritBuff =>
        IsActive || Game1.buffsDisplay.otherBuffs.Any(b => b.which == BuffId - 4);

    internal double SecondsOutOfAmbush { get; set; } = double.MaxValue;

    #endregion internal properties

    #region internal methods

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();

        SecondsOutOfAmbush = 0d;

        foreach (var monster in Game1.currentLocation.characters.OfType<Monster>()
                     .Where(m => m.Player?.IsLocalPlayer == true))
        {
            monster.focusedOnFarmers = false;
            switch (monster)
            {
                case AngryRoger:
                case Ghost:
                    ModEntry.ModHelper.Reflection.GetField<bool>(monster, "seenPlayer").SetValue(false);
                    break;
                case Bat:
                case RockGolem:
                    ModEntry.ModHelper.Reflection.GetField<NetBool>(monster, "seenPlayer").GetValue().Value = false;
                    break;
                case DustSpirit:
                    ModEntry.ModHelper.Reflection.GetField<bool>(monster, "seenFarmer").SetValue(false);
                    ModEntry.ModHelper.Reflection.GetField<bool>(monster, "chargingFarmer").SetValue(false);
                    break;
                case ShadowGuy:
                case ShadowShaman:
                case Skeleton:
                    ModEntry.ModHelper.Reflection.GetField<bool>(monster, "spottedPlayer").SetValue(false);
                    break;
            }
        }

        var critBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == BuffId - 4);
        var duration = critBuff?.millisecondsDuration ?? MillisecondsDuration;

        Game1.buffsDisplay.removeOtherBuff(BuffId - 4);
        Game1.buffsDisplay.removeOtherBuff(BuffId);
        Game1.player.addedSpeed -= 2;
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                GetType().Name,
                ModEntry.i18n.Get("poacher.ulti.name"))
            {
                which = BuffId,
                sheetIndex = 49,
                glow = GlowColor,
                millisecondsDuration = duration,
                description = ModEntry.i18n.Get("poacher.ulti.desc.hidden")
            }
        );

        ModEntry.Events.Enable<AmbushUpdateTickedEvent>();
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == BuffId);
        var timeLeft = buff?.millisecondsDuration ?? 0;
        Game1.buffsDisplay.removeOtherBuff(BuffId);
        Game1.player.addedSpeed += 2;
        if (timeLeft < 100) return;

        var buffId = BuffId - 4;
        Game1.buffsDisplay.removeOtherBuff(buffId);
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                GetType().Name,
                ModEntry.i18n.Get("poacher.ulti.name"))
            {
                which = buffId,
                sheetIndex = 37,
                millisecondsDuration = timeLeft * 2,
                description = ModEntry.i18n.Get("poacher.ulti.desc.revealed")
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