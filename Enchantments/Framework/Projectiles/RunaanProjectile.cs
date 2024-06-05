namespace DaLion.Enchantments.Framework.Projectiles;

#region using directives

using System.Linq;
using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>An echo of a <see cref="BasicProjectile"/> fired by a <see cref="Slingshot"/> enchanted with <see cref="RunaanEnchantment"/>.</summary>
internal sealed class RunaanProjectile : BasicProjectile
{
    private readonly BasicProjectile _original;
    private int _timer;

    /// <summary>Initializes a new instance of the <see cref="RunaanProjectile"/> class.</summary>
    /// <param name="original">The original <see cref="BasicProjectile"/> that will be echoed.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired the shot.</param>
    public RunaanProjectile(BasicProjectile original, Vector2 startingPosition, GameLocation location, Farmer firer)
    : base(
        (int)(original.damageToFarmer.Value * 0.6),
        original.currentTileSheetIndex.Value,
        0,
        original.tailLength.Value,
        original.rotationVelocity.Value,
        0f,
        0f,
        startingPosition,
        original.collisionSound.Value,
        null,
        null,
        false,
        original.damagesMonsters.Value,
        location,
        firer,
        null,
        original.itemId.Value)
    {
        this._original = original;
        this._timer = 750 + (Game1.random.Next(-25, 26) * 10); // delay before motion
    }

    /// <inheritdoc />
    public override bool update(GameTime time, GameLocation location)
    {
        if (this._timer <= 0)
        {
            return base.update(time, location);
        }

        this._timer -= time.ElapsedGameTime.Milliseconds;
        if (this._timer > 0)
        {
            return base.update(time, location);
        }

        var speed = new Vector2(this._original.xVelocity.Value, this._original.yVelocity.Value).Length();
        var targetCandidates = location.characters.OfType<Monster>().ToList();
        var chosenTarget = this.position.Value.GetClosest(targetCandidates, monster => monster.Position, out _);
        var targetDirection = chosenTarget?.GetBoundingBox().Center.ToVector2() - this.position.Value - new Vector2(32f, 32f) ??
                              Utility.getVelocityTowardPoint(
                                  this.position.Value,
                                  this._original.position.Value,
                                  speed);
        targetDirection.Normalize();

        var velocity = targetDirection * speed;
        this.xVelocity.Value = velocity.X;
        this.yVelocity.Value = velocity.Y;
        return base.update(time, location);
    }
}
