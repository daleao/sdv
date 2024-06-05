namespace DaLion.Enchantments;

#region using directives

using DaLion.Enchantments.Framework.Projectiles;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;

#endregion using directives

/// <summary>The <see cref="EnchantmentsMod"/> API implementation.</summary>
public class EnchantmentsApi : IEnchantmentsApi
{
    /// <inheritdoc />
    public BasicProjectile CreateQuincyProjectile(
        Farmer firer, Vector2 startingPosition, float xVelocity, float yVelocity, float rotationVelocity, float scale)
    {
        return new QuincyProjectile(firer, startingPosition, xVelocity, yVelocity, rotationVelocity, scale);
    }
}
