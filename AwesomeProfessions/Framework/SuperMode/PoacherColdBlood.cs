// ReSharper disable PossibleLossOfFraction

using DaLion.Stardew.Professions.Framework.Events.GameLoop;

namespace DaLion.Stardew.Professions.Framework.SuperMode;

#region using directives

using System.Linq;
using Netcode;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

using AssetLoaders;

#endregion using directives

/// <summary>Handles Poacher Cold Blood activation.</summary>
internal sealed class PoacherColdBlood : SuperMode
{
    /// <summary>Construct an instance.</summary>
    internal PoacherColdBlood()
    {
        Gauge = new(this, Color.MediumPurple);
        Overlay = new(Color.MidnightBlue);
        EnableEvents();
    }

    #region public properties

    public override SuperModeIndex Index => SuperModeIndex.Poacher;
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
            }
        }

        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) SuperModeIndex.Poacher + 4;
        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == buffId);
        if (buff is null)
        {
            Game1.buffsDisplay.addOtherBuff(
                new(0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    1,
                    GetType().Name,
                    ModEntry.ModHelper.Translation.Get("poacher.superm"))
                {
                    which = buffId,
                    sheetIndex = 49,
                    glow = GlowColor,
                    millisecondsDuration = (int) (SuperMode.MaxValue * ModEntry.Config.SuperModeDrainFactor * 10),
                    description = ModEntry.ModHelper.Translation.Get("poacher.supermdesc")
                }
            );
        }

        if (Context.IsMainPlayer)
            ModEntry.HostState.PoachersInAmbush.Add(Game1.player.UniqueMultiplayerID);
        else
            ModEntry.ModHelper.Multiplayer.SendMessage("ActivatedAmbush", "RequestUpdateHostState",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
    }

    /// <inheritdoc />
    public override void Deactivate()
    {
        base.Deactivate();
        if (Context.IsMainPlayer)
            ModEntry.HostState.PoachersInAmbush.Remove(Game1.player.UniqueMultiplayerID);
        else
            ModEntry.ModHelper.Multiplayer.SendMessage("DeactivatedAmbush", "RequestUpdateHostState",
                new[] {ModEntry.Manifest.UniqueID}, new[] {Game1.MasterPlayer.UniqueMultiplayerID});
    }

    /// <inheritdoc />
    public override void AddBuff()
    {
        if (ChargeValue < 10.0) return;

        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) SuperModeIndex.Poacher;
        var magnitude = GetCritDamageMultiplier().ToString("0.0");
        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == buffId);
        if (buff == null)
            Game1.buffsDisplay.addOtherBuff(
                new(0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    1,
                    GetType().Name,
                    ModEntry.ModHelper.Translation.Get("poacher.buff"))
                {
                    which = buffId,
                    sheetIndex = 37,
                    millisecondsDuration = 0,
                    description = ModEntry.ModHelper.Translation.Get("poacher.buffdesc", new {magnitude})
                });
    }

    /// <summary>The multiplier applied to critical damage performed by Poacher.</summary>
    public float GetCritDamageMultiplier()
    {
        return IsActive
            ? 1f + MaxValue / 10 * 0.04f // apply the maximum cold blood bonus
            : 1f + (int) ChargeValue / 10 * 0.04f; // apply current cold blood bonus
    }

    #endregion public methods
}