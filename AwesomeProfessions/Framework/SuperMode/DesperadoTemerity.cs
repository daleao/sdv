namespace DaLion.Stardew.Professions.Framework.SuperMode;

#region using directives

using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;

using AssetLoaders;
using Extensions;

#endregion using directives

/// <summary>Handles Desperado Temerity activation.</summary>
internal sealed class DesperadoTemerity : SuperMode
{
    /// <summary>Construct an instance.</summary>
    internal DesperadoTemerity()
    {
        Gauge = new(this, Color.DarkGoldenrod);
        Overlay = new(Color.SandyBrown);
        EnableEvents();
    }

    #region public properties

    public override SFX ActivationSfx => SFX.DesperadoBlossom;
    public override Color GlowColor => Color.DarkGoldenrod;
    public override SuperModeIndex Index => SuperModeIndex.Desperado;

    #endregion public properties

    #region public methods

    /// <inheritdoc />
    public override void Activate()
    {
        base.Activate();

        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) SuperModeIndex.Desperado + 4;
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
                    ModEntry.ModHelper.Translation.Get("desperado.superm"))
                {
                    which = buffId,
                    sheetIndex = 51,
                    glow = GlowColor,
                    millisecondsDuration = (int) (SuperMode.MaxValue * ModEntry.Config.SuperModeDrainFactor * 10),
                    description = ModEntry.ModHelper.Translation.Get("desperado.supermdesc")
                }
            );
        }
    }

    /// <inheritdoc />
    public override void AddBuff()
    {
        if (ChargeValue < 10.0) return;

        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) SuperModeIndex.Desperado;
        var magnitude = GetShootingPower().ToString("0.0");
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
                    ModEntry.ModHelper.Translation.Get("desperado.buff"))
                {
                    which = buffId,
                    sheetIndex = 39,
                    millisecondsDuration = 0,
                    description = ModEntry.ModHelper.Translation.Get("desperado.buffdesc", new {magnitude})
                });
    }

    /// <summary>The Desperado's shooting power, which affects charging speed, projectile velocity, knock-back, hit-box and pierce chance.</summary>
    public float GetShootingPower()
    {
        return IsActive
            ? 1f // disable temerity bonus in super mode
            : 1f + (float) ChargeValue / 10 * 0.01f; // apply the current temerity bonus
    }

    #endregion public methods
}