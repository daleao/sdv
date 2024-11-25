namespace DaLion.Core.Framework.Extensions;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Extensions for the <see cref="SpriteBatch"/> class.</summary>
public static class SpriteBatchExtensions
{
    /// <summary>Draws special move cooldown icon.</summary>
    /// <param name="batch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> position.</param>
    public static void DrawCooldownIcon(this SpriteBatch batch, Vector2 position)
    {
        Utility.drawWithShadow(
            batch,
            Textures.TooltipsTx,
            position,
            new Rectangle(10, 0, 10, 10),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            false,
            1f);
    }

    /// <summary>Draws light-bulb icon.</summary>
    /// <param name="batch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> position.</param>
    public static void DrawLightIcon(this SpriteBatch batch, Vector2 position)
    {
        Utility.drawWithShadow(
            batch,
            Textures.TooltipsTx,
            position,
            new Rectangle(0, 0, 10, 10),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            false,
            1f);
    }
}
