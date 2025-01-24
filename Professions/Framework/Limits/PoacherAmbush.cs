namespace DaLion.Professions.Framework.Limits;

#region using directives

using DaLion.Professions.Framework.Buffs;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PoacherAmbush"/> class.</summary>
public sealed class PoacherAmbush()
    : LimitBreak(Profession.Poacher, "Ambush", Color.MediumPurple, Color.MidnightBlue)
{
    /// <summary>Gets or sets the number of seconds since deactivation.</summary>
    internal double SecondsOutOfAmbush { get; set; } = double.MaxValue;

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();
        this.SecondsOutOfAmbush = 0d;
        Game1.player.buffs.AppliedBuffs.Remove(PoacherBackstabBuff.ID);
        Game1.player.applyBuff(new PoacherAmbushBuff());
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();
        var timeLeft = Game1.player.buffs.AppliedBuffs.TryGetValue(PoacherAmbushBuff.ID, out var buff)
            ? buff.millisecondsDuration
            : 0;

        Game1.player.buffs.AppliedBuffs.Remove(PoacherAmbushBuff.ID);
        if (timeLeft < 1000)
        {
            return;
        }

        Game1.player.applyBuff(new PoacherBackstabBuff(timeLeft * 2));
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        // base duration 30 s * 60 fps = 1800 frames
        this.ChargeValue -= BASE_MAX_CHARGE / 1800d;
    }
}
