namespace DaLion.Stardew.Rings.Framework;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures.</summary>
public static class Textures
{
    public static Texture2D GemstonesTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/Gemstones");
}