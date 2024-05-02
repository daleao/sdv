namespace DaLion.Professions.Framework.Limits;

#region using directives

using DaLion.Professions.Framework.Buffs;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Handles Poacher Limit Break activation.</summary>
public sealed class PoacherAmbush : LimitBreak
{
    /// <summary>Initializes a new instance of the <see cref="PoacherAmbush"/> class.</summary>
    internal PoacherAmbush()
        : base(Profession.Poacher, "Ambush", Color.MediumPurple, Color.MidnightBlue)
    {
    }

    /// <summary>Gets or sets the number of seconds since deactivation.</summary>
    internal double SecondsOutOfAmbush { get; set; } = double.MaxValue;

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();
        this.SecondsOutOfAmbush = 0d;
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
        if (timeLeft < 100)
        {
            return;
        }

        Game1.player.applyBuff(new PoacherBackstabBuff(timeLeft * 2));
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        this.ChargeValue -= MaxCharge / 900d; // lasts 15s * 60 ticks/s -> 900 ticks
    }
}
