// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Framework.Ultimates;

#region using directives

using Events.GameLoop;
using Microsoft.Xna.Framework;
using Netcode;
using Sounds;
using StardewValley;
using StardewValley.Monsters;
using System.Linq;

#endregion using directives

/// <summary>Handles Poacher ultimate activation.</summary>
public sealed class Ambush : Ultimate
{
    /// <summary>Construct an instance.</summary>
    internal Ambush()
    : base(UltimateIndex.PoacherAmbush, Color.MediumPurple, Color.MidnightBlue) { }

    #region public properties

    /// <summary>The ID of the buff that displays while Ambush is active.</summary>
    public static int BuffId { get; } = (ModEntry.Manifest.UniqueID + (int)UltimateIndex.PoacherAmbush + 4).GetHashCode();

    #endregion public properties

    #region internal properties

    /// <inheritdoc />
    internal override SFX ActivationSfx => SFX.PoacherAmbush;

    /// <inheritdoc />
    internal override Color GlowColor => Color.MediumPurple;

    /// <summary>Whether the double crit. power buff is active.</summary>
    internal bool IsGrantingCritBuff =>
        IsActive || Game1.buffsDisplay.otherBuffs.Any(b => b.which == BuffId - 4);

    internal double SecondsOutOfAmbush { get; set; }

    #endregion internal properties

    #region internal methods

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();

        SecondsOutOfAmbush = 0d;
        foreach (var monster in Game1.currentLocation.characters.OfType<Monster>()
                     .Where(m => m.Player.IsLocalPlayer))
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
        var duration = critBuff?.millisecondsDuration ??
                       (int)(15000 * ((double)MaxValue / BASE_MAX_VALUE_I) / ModEntry.Config.SpecialDrainFactor);
        Game1.buffsDisplay.removeOtherBuff(BuffId - 4);
        Game1.buffsDisplay.removeOtherBuff(BuffId);
        Game1.player.addedSpeed -= 2;
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                GetType().Name,
                ModEntry.i18n.Get("poacher.ulti"))
            {
                which = BuffId,
                sheetIndex = 49,
                glow = GlowColor,
                millisecondsDuration = duration,
                description = ModEntry.i18n.Get("poacher.ultidesc.hidden")
            }
        );

        ModEntry.EventManager.Enable<AmbushUpdateTickedEvent>();
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == BuffId);
        var timeLeft = buff?.millisecondsDuration ?? 0;
        Game1.buffsDisplay.removeOtherBuff(BuffId);
        Game1.player.addedSpeed += 2;
        if (timeLeft <= 0) return;

        var buffId = BuffId - 4;
        Game1.buffsDisplay.removeOtherBuff(buffId);
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                GetType().Name,
                ModEntry.i18n.Get("poacher.ulti"))
            {
                which = buffId,
                sheetIndex = 37,
                millisecondsDuration = timeLeft * 2,
                description = ModEntry.i18n.Get("poacher.ultidesc.revealed")
            }
        );
    }

    /// <inheritdoc />
    internal override void Countdown(double elapsed)
    {
        ChargeValue -= elapsed * 0.02 / 3.0; // lasts 15s
    }

    /// <summary>Reset the Ambush timer with twice the remaining duration.</summary>
    internal void Reset()
    {
        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == BuffId);
        if (buff is null) return;

        buff.millisecondsDuration *= 2;
    }

    #endregion internal methods
}