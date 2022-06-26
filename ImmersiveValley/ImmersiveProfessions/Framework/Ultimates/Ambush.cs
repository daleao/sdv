// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Framework.Ultimates;

#region using directives

using Microsoft.Xna.Framework;
using Netcode;
using Sounds;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System.Linq;

#endregion using directives

/// <summary>Handles Poacher ultimate activation.</summary>
internal sealed class Ambush : Ultimate
{
    /// <summary>Construct an instance.</summary>
    internal Ambush()
    : base(Color.MediumPurple, Color.MidnightBlue)
    {
    }

    #region public properties

    /// <summary>The ID of the buff that displays while Ambush is active.</summary>
    public static int BuffId { get; } = (ModEntry.Manifest.UniqueID + (int)UltimateIndex.PoacherAmbush + 4).GetHashCode();

    /// <inheritdoc />
    public override UltimateIndex Index => UltimateIndex.PoacherAmbush;

    #endregion public properties

    #region internal properties

    /// <inheritdoc />
    internal override SFX ActivationSfx => SFX.PoacherAmbush;

    /// <inheritdoc />
    internal override Color GlowColor => Color.MediumPurple;

    #endregion internal properties

    #region internal methods

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();

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

        Game1.player.addedSpeed -= 3;
        Game1.buffsDisplay.removeOtherBuff(BuffId);
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                GetType().Name,
                ModEntry.i18n.Get("poacher.ulti"))
            {
                which = BuffId,
                sheetIndex = 49,
                glow = GlowColor,
                millisecondsDuration = (int)(30000 * ((double)MaxValue / BASE_MAX_VALUE_I) / ModEntry.Config.SpecialDrainFactor),
                description = ModEntry.i18n.Get("poacher.ultidesc.hidden")
            }
        );

        if (Context.IsMainPlayer)
            ModEntry.HostState.PoachersInAmbush.Add(Game1.player.UniqueMultiplayerID);
        else
            ModEntry.Broadcaster.Message("ActivatedAmbush", "RequestUpdateHostState", Game1.MasterPlayer.UniqueMultiplayerID);
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == BuffId);
        var timeLeft = buff?.millisecondsDuration ?? 0;
        Game1.buffsDisplay.removeOtherBuff(BuffId);

        Game1.player.addedSpeed += 3;

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
                millisecondsDuration = 2 * timeLeft,
                description = ModEntry.i18n.Get("poacher.ultidesc.revealed")
            }
        );

        if (Context.IsMainPlayer)
            ModEntry.HostState.PoachersInAmbush.Remove(Game1.player.UniqueMultiplayerID);
        else
            ModEntry.Broadcaster.Message("DeactivatedAmbush", "RequestUpdateHostState", Game1.MasterPlayer.UniqueMultiplayerID);

    }

    /// <inheritdoc />
    internal override void Countdown(double elapsed)
    {
        ChargeValue -= elapsed * 0.06 / 18.0;
    }

    /// <summary>Whether the double crit. power buff is active.</summary>
    internal bool ShouldBuffCritPower() =>
        IsActive || Game1.buffsDisplay.otherBuffs.Any(b => b.which == BuffId - 4);

    #endregion internal methods
}