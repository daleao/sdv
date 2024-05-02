namespace DaLion.Professions.Framework.Limits;

#region using directives

using DaLion.Professions.Framework.Buffs;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Handles Brute Limit Break activation.</summary>
public sealed class BruteFrenzy : LimitBreak
{
    /// <summary>Initializes a new instance of the <see cref="BruteFrenzy"/> class.</summary>
    internal BruteFrenzy()
        : base(Profession.Brute, "Frenzy", Color.OrangeRed, Color.OrangeRed)
    {
    }

    /// <summary>Gets or sets the number of enemies defeated while active.</summary>
    internal int KillCount { get; set; }

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();
        this.KillCount = 0;
        Game1.player.applyBuff(new BruteFrenzyBuff());
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();
        Game1.player.buffs.AppliedBuffs.Remove(PoacherAmbushBuff.ID);

        var who = Game1.player;
        var healed = (int)(who.maxHealth * this.KillCount * 0.05f);
        who.health = Math.Min(who.health + healed, who.maxHealth);
        who.currentLocation.debris.Add(new Debris(
            healed,
            new Vector2(who.StandingPixel.X + 8, who.StandingPixel.Y),
            Color.Lime,
            1f,
            who));
        Game1.playSound("healSound");
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        this.ChargeValue -= MaxCharge / 900d; // lasts 15s * 60 ticks/s -> 900 ticks
    }
}
