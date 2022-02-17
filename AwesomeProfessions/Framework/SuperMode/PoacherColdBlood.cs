namespace DaLion.Stardew.Professions.Framework.SuperMode;

#region using directives

using System.Linq;
using Netcode;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

using AssetLoaders;
using Extensions;

#endregion using directives

/// <summary>Handles Poacher Cold Blood activation.</summary>
internal sealed class PoacherColdBlood : SuperMode
{
    /// <summary>Construct an instance.</summary>
    internal PoacherColdBlood()
    {
        Gauge = new(Color.MediumPurple);
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
                case DustSpirit:
                case Ghost:
                    ModEntry.ModHelper.Reflection.GetField<bool>(monster, "chargingFarmer").SetValue(false);
                    ModEntry.ModHelper.Reflection.GetField<bool>(monster, "seenFarmer").SetValue(false);
                    break;

                case Bat:
                case RockGolem:
                    ModEntry.ModHelper.Reflection.GetField<NetBool>(monster, "seenPlayer").GetValue().Set(false);
                    break;
            }
        }

        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int)SuperModeIndex.Poacher + 4;
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
                    millisecondsDuration = (int) (SuperModeGauge.MaxValue * ModEntry.Config.SuperModeDrainFactor * 10),
                    description = ModEntry.ModHelper.Translation.Get("poacher.supermdesc")
                }
            );
        }
    }

    /// <inheritdoc />
    public override void AddBuff()
    {
        if (Gauge.CurrentValue < 10.0) return;

        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) SuperModeIndex.Poacher;
        var magnitude = Game1.player.GetPoacherCritDamageMultiplier().ToString("0.0");
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

    #endregion public methods
}