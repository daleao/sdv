namespace DaLion.Professions.Framework.Limits;

#region using directives

using DaLion.Professions.Framework.Buffs;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DesperadoBlossom"/> class.</summary>
public sealed class DesperadoBlossom()
    : LimitBreak(Profession.Desperado, "Blossom", Color.DarkGoldenrod, Color.SandyBrown)
{
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
        // base duration 15 s * 60 fps = 900 frames
        this.ChargeValue -= BASE_MAX_CHARGE / 900d;
    }
}
