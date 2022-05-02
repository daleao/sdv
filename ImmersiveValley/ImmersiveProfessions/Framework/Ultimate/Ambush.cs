// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Framework.Ultimate;

#region using directives

using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

using Sounds;

#endregion using directives

/// <summary>Handles Poacher ultimate activation.</summary>
internal sealed class Ambush : Ultimate
{
    /// <summary>Construct an instance.</summary>
    internal Ambush()
    {
        Meter = new(this, Color.MediumPurple);
        Overlay = new(Color.MidnightBlue);
    }

    #region public properties

    public static int BuffId { get; } = ModEntry.Manifest.UniqueID.GetHashCode() + (int) UltimateIndex.Poacher + 4;

    public override UltimateIndex Index => UltimateIndex.Poacher;
    public override SFX ActivationSfx => SFX.PoacherAmbush;
    public override Color GlowColor => Color.MediumPurple;

    #endregion public properties

    #region public methods

    /// <inheritdoc />
    public override void Activate()
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
                ModEntry.ModHelper.Translation.Get("poacher.ulti"))
            {
                which = BuffId,
                sheetIndex = 49,
                glow = GlowColor,
                millisecondsDuration = (int) (30000 * ((double) MaxValue / BASE_MAX_VALUE_I) / ModEntry.Config.UltimateDrainFactor),
                description = ModEntry.ModHelper.Translation.Get("poacher.ultidesc.hidden")
            }
        );

        if (Context.IsMainPlayer)
            ModEntry.HostState.PoachersInAmbush.Add(Game1.player.UniqueMultiplayerID);
        else
            ModEntry.ModHelper.Multiplayer.SendMessage("ActivatedAmbush", "RequestUpdateHostState",
                new[] {ModEntry.Manifest.UniqueID}, new[] {Game1.MasterPlayer.UniqueMultiplayerID});
    }

    /// <inheritdoc />
    public override void Deactivate()
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
                ModEntry.ModHelper.Translation.Get("poacher.ulti"))
            {
                which = buffId,
                sheetIndex = 37,
                millisecondsDuration = 2 * timeLeft,
                description = ModEntry.ModHelper.Translation.Get("poacher.ultidesc.revealed")
            }
        );

        if (Context.IsMainPlayer)
            ModEntry.HostState.PoachersInAmbush.Remove(Game1.player.UniqueMultiplayerID);
        else
            ModEntry.ModHelper.Multiplayer.SendMessage("DeactivatedAmbush", "RequestUpdateHostState",
                new[] {ModEntry.Manifest.UniqueID}, new[] {Game1.MasterPlayer.UniqueMultiplayerID});

    }

    /// <inheritdoc />
    public override void Countdown(double elapsed)
    {
        ChargeValue -= elapsed * 0.06 / 18.0;
    }

    /// <summary>Whether the double crit. power buff is active.</summary>
    public bool ShouldBuffCritPower()
    {
        return IsActive || Game1.buffsDisplay.otherBuffs.Any(b => b.which == BuffId - 4);
    }

    #endregion public methods
}