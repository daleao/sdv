namespace DaLion.Professions.Framework.Limits;

#region using directives

using System.Linq;
using DaLion.Professions.Framework.Buffs;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PiperConcerto"/> class.</summary>
public sealed class PiperConcerto()
    : LimitBreak(Profession.Piper, "Concerto", Color.LimeGreen, Color.DarkGreen)
{
    /// <inheritdoc />
    public override bool CanActivate => base.CanActivate && Game1.player.currentLocation.characters.OfType<Monster>()
        .Any(m => m.IsSlime() && m.IsCharacterWithinThreshold());

    /// <summary>Gets or sets the number of ticks since the latest contact with a Slime.</summary>
    internal int SlimeContactTimer { get; set; }

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();
        Game1.player.applyBuff(new PiperConcertoBuff());
        Game1.player.FarmerSprite.animateOnce(
        [
            new FarmerSprite.AnimationFrame(98, 400, secondaryArm: true, flip: false),
            new FarmerSprite.AnimationFrame(99, 200, secondaryArm: true, flip: false),
            new FarmerSprite.AnimationFrame(100, 200, secondaryArm: true, flip: false),
            new FarmerSprite.AnimationFrame(99, 200, secondaryArm: true, flip: false),
            new FarmerSprite.AnimationFrame(98, 400, secondaryArm: true, flip: false),
            new FarmerSprite.AnimationFrame(99, 200, secondaryArm: true, flip: false)
        ]);

        Game1.player.freezePause = 1500;
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();
        Game1.player.buffs.AppliedBuffs.Remove(PiperConcertoBuff.ID);
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        // base duration 30 s * 60 fps = 1800 frames
        this.ChargeValue -= BASE_MAX_CHARGE / 900d;
    }
}
