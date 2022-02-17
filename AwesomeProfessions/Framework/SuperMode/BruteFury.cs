namespace DaLion.Stardew.Professions.Framework.SuperMode;

#region using directives

using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;

using AssetLoaders;
using Extensions;

#endregion using directives

/// <summary>Handles Brute Fury activation.</summary>
internal sealed class BruteFury : SuperMode
{
    /// <summary>Construct an instance.</summary>
    internal BruteFury()
    {
        Gauge = new(Color.OrangeRed);
        Overlay = new(Color.OrangeRed);
        EnableEvents();
    }

    #region public properties

    public override SFX ActivationSfx => SFX.BruteRage;
    public override Color GlowColor => Color.OrangeRed;
    public override SuperModeIndex Index => SuperModeIndex.Brute;

    #endregion public properties

    #region public methods

    /// <inheritdoc />
    public override void Activate()
    {
        base.Activate();

        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int)SuperModeIndex.Brute + 4;
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
                    ModEntry.ModHelper.Translation.Get("brute.superm"))
                {
                    which = buffId,
                    sheetIndex = 48,
                    glow = GlowColor,
                    millisecondsDuration = (int) (SuperModeGauge.MaxValue * ModEntry.Config.SuperModeDrainFactor * 10),
                    description = ModEntry.ModHelper.Translation.Get("brute.supermdesc")
                }
            );
        }
    }

    /// <inheritdoc />
    public override void AddBuff()
    {
        if (Gauge.CurrentValue < 10.0) return;

        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) SuperModeIndex.Brute;
        var magnitude = ((Game1.player.GetBruteBonusDamageMultiplier() - 1.15) * 100f).ToString("0.0");
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
                    ModEntry.ModHelper.Translation.Get("brute.buff"))
                {
                    which = buffId,
                    sheetIndex = 36,
                    millisecondsDuration = 0,
                    description = ModEntry.ModHelper.Translation.Get("brute.buffdesc", new {magnitude})
                });
    }

    #endregion public methods
}