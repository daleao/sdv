namespace DaLion.Enchantments.Framework.Projectiles;

#region using directives

using DaLion.Enchantments.Framework.Enchantments;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>An energy projectile fired by a <see cref="Slingshot"/> which has the <see cref="QuincyEnchantment"/>.</summary>
internal sealed class QuincyProjectile : BasicProjectile
{
    public const int BLUE_SHEET_INDEX = 14;
    public const int YELLOW_SHEET_INDEX = 20;
    public const int RED_SHEET_INDEX = 21;

    /// <summary>Initializes a new instance of the <see cref="QuincyProjectile"/> class.</summary>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="xVelocity">The projectile's starting velocity in the horizontal direction.</param>
    /// <param name="yVelocity">The projectile's starting velocity in the vertical direction.</param>
    /// <param name="rotationVelocity">The projectile's starting rotational velocity.</param>
    /// <param name="scale">The projectile's starting scale.</param>
    public QuincyProjectile(
        Farmer firer,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity,
        float scale)
        : base(
            1,
            BLUE_SHEET_INDEX,
            0,
            5,
            rotationVelocity,
            xVelocity,
            yVelocity,
            startingPosition,
            "debuffHit",
            null,
            null,
            true,
            true,
            firer.currentLocation,
            firer)
    {
        this.damageToFarmer.Value = 30;
        if (firer.health < firer.maxHealth * 1f / 3f)
        {
            this.currentTileSheetIndex.Value = RED_SHEET_INDEX;
            this.damageToFarmer.Value *= 3;
        }
        else if (firer.health < firer.maxHealth * 2f / 3f)
        {
            this.currentTileSheetIndex.Value = YELLOW_SHEET_INDEX;
            this.damageToFarmer.Value *= 2;
        }

        this.startingScale.Value = scale;
        this.IgnoreLocationCollision = true;
        this.ignoreTravelGracePeriod.Value = true;
    }
}
