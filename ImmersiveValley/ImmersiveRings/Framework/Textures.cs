namespace DaLion.Stardew.Rings.Framework;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures.</summary>
public static class Textures
{
    /// <summary>Gets the <see cref="Texture2D"/> used to draw <see cref="Gemstone"/>s onto Infinity Bands.</summary>
    public static Texture2D GemstonesTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/Gemstones");

    /// <summary>Gets the <see cref="Texture2D"/> used to draw <see cref="Gemstone"/>s resonance <see cref="LightSource"/>s.</summary>
    public static Texture2D ResonanceLightTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/Light");
}
