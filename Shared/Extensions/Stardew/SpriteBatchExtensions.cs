namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Extensions for the <see cref="SpriteBatch"/> class.</summary>
public static class SpriteBatchExtensions
{
    /// <summary>Draws attack icon.</summary>
    /// <param name="batch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> position.</param>
    public static void DrawAttackIcon(this SpriteBatch batch, Vector2 position)
    {
        Utility.drawWithShadow(
            batch,
            Game1.mouseCursors,
            position,
            new Rectangle(120, 428, 10, 10),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            false,
            1f);
    }

    /// <summary>Draws defense icon.</summary>
    /// <param name="batch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> position.</param>
    public static void DrawDefenseIcon(this SpriteBatch batch, Vector2 position)
    {
        Utility.drawWithShadow(
            batch,
            Game1.mouseCursors,
            position,
            new Rectangle(110, 428, 10, 10),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            false,
            1f);
    }

    /// <summary>Draws weight icon.</summary>
    /// <param name="batch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> position.</param>
    public static void DrawWeightIcon(this SpriteBatch batch, Vector2 position)
    {
        Utility.drawWithShadow(
            batch,
            Game1.mouseCursors,
            position,
            new Rectangle(70, 428, 10, 10),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            false,
            1f);
    }

    /// <summary>Draws critical hit chance icon.</summary>
    /// <param name="batch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> position.</param>
    public static void DrawCritChanceIcon(this SpriteBatch batch, Vector2 position)
    {
        Utility.drawWithShadow(
            batch,
            Game1.mouseCursors,
            position,
            new Rectangle(40, 428, 10, 10),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            false,
            1f);
    }

    /// <summary>Draws critical hit power icon.</summary>
    /// <param name="batch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> position.</param>
    public static void DrawCritPowerIcon(this SpriteBatch batch, Vector2 position)
    {
        Utility.drawWithShadow(
            batch,
            Game1.mouseCursors,
            position,
            new Rectangle(160, 428, 10, 10),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            false,
            1f);
    }

    /// <summary>Draws attack speed icon.</summary>
    /// <param name="batch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> position.</param>
    public static void DrawSpeedIcon(this SpriteBatch batch, Vector2 position)
    {
        Utility.drawWithShadow(
            batch,
            Game1.mouseCursors,
            position,
            new Rectangle(130, 428, 10, 10),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            false,
            1f);
    }

    /// <summary>Draws immunity icon.</summary>
    /// <param name="batch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> position.</param>
    public static void DrawImmunityIcon(this SpriteBatch batch, Vector2 position)
    {
        Utility.drawWithShadow(
            batch,
            Game1.mouseCursors,
            position,
            new Rectangle(150, 428, 10, 10),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            false,
            1f);
    }

    /// <summary>Draws magnetism icon.</summary>
    /// <param name="batch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> position.</param>
    public static void DrawMagnetismIcon(this SpriteBatch batch, Vector2 position)
    {
        Utility.drawWithShadow(
            batch,
            Game1.mouseCursors,
            position,
            new Rectangle(90, 428, 10, 10),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            false,
            1f);
    }

    /// <summary>Draws enchantment icon.</summary>
    /// <param name="batch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> position.</param>
    public static void DrawEnchantmentIcon(this SpriteBatch batch, Vector2 position)
    {
        Utility.drawWithShadow(
            batch,
            Game1.mouseCursors2,
            position,
            new Rectangle(127, 35, 10, 10),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            false,
            1f);
    }
}
