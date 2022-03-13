namespace DaLion.Stardew.Tweaks.Framework;

#region using directives

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    #region textures

    internal static Texture2D HoneyMeadTx { get; set; } =
        Game1.content.Load<Texture2D>(Path.Combine(ModEntry.Manifest.UniqueID, "BetterHoneyMeadIcons"));

    #endregion textures

    internal static bool TryGetSourceRectForMead(int preserveIndex, out Rectangle sourceRectangle)
    {
        sourceRectangle = preserveIndex switch
        {
            376 => new(48, 0, 16, 16), // poppy
            402 => new(0, 16, 16, 16), // sweet pea
            418 => new(16, 16, 16, 16), // crocus
            421 => new(32, 0, 16, 16), // sunflower
            591 => new(0, 0, 16, 16), // tulip
            593 => new(32, 0, 16, 16), // summer spangle
            595 => new(64, 0, 16, 16), // fairy rose
            597 => new(16, 0, 16, 16), // blue jazz
            _ => new(32, 16, 16, 16)
        };

        return sourceRectangle != default;
    }
}