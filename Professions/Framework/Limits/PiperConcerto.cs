namespace DaLion.Professions.Framework.Limits;

#region using directives

using System.Linq;
using DaLion.Professions.Framework.Buffs;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Handles Piper Limit Break activation.</summary>
public sealed class PiperConcerto : LimitBreak
{
    /// <summary>Initializes a new instance of the <see cref="PiperConcerto"/> class.</summary>
    internal PiperConcerto()
        : base(Profession.Piper, "Concerto", Color.LimeGreen, Color.DarkGreen)
    {
    }

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
        this.ChargeValue -= MaxCharge / 1800d; // lasts 30s * 60 ticks/s -> 1800 ticks
    }
}
