namespace DaLion.Stardew.Rings.Framework;

#region using directives

using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

#endregion using directives

/// <summary>Caches custom mod textures.</summary>
public static class Textures
{
    public static Texture2D GemstonesTx { get; } =
        Game1.content.Load<Texture2D>(Path.Combine(ModEntry.Manifest.UniqueID, "Gemstones"));
}