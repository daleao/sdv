namespace DaLion.Enchantments;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley.Projectiles;

#endregion using directives

/// <summary>The public interface for the Taxes mod API.</summary>
public interface IEnchantmentsApi
{
    /// <summary>Creates a new instance of <see cref="Framework.Projectiles.QuincyProjectile"/>.</summary>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="xVelocity">The projectile's starting velocity in the horizontal direction.</param>
    /// <param name="yVelocity">The projectile's starting velocity in the vertical direction.</param>
    /// <param name="rotationVelocity">The projectile's starting rotational velocity.</param>
    /// <param name="scale">The projectile's starting scale.</param>
    /// <returns>A new instance of a <see cref="Framework.Projectiles.QuincyProjectile"/>.</returns>
    BasicProjectile CreateQuincyProjectile(
        Farmer firer, Vector2 startingPosition, float xVelocity, float yVelocity, float rotationVelocity, float scale);
}
