namespace DaLion.Professions.Framework.Limits;

#region using directives

using DaLion.Professions.Framework.Buffs;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Handles Desperado Limit Break activation.</summary>
public sealed class DesperadoBlossom : LimitBreak
{
    /// <summary>Initializes a new instance of the <see cref="DesperadoBlossom"/> class.</summary>
    internal DesperadoBlossom()
        : base(Profession.Desperado, "Blossom", Color.DarkGoldenrod, Color.SandyBrown)
    {
    }

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();
        Game1.player.applyBuff(new DesperadoBlossomBuff());
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();
        Game1.player.buffs.AppliedBuffs.Remove(DesperadoBlossomBuff.ID);
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        this.ChargeValue -= MaxCharge / 900d; // lasts 15s * 60 ticks/s -> 900 ticks
    }
}
