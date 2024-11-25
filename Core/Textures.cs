namespace DaLion.Core;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

public static class Textures
{
    private static readonly Lazy<Texture2D> _tooltipsTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/sprites/tooltips"));

    public static Texture2D TooltipsTx => _tooltipsTx.Value;
}
