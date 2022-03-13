namespace DaLion.Stardew.Professions.Framework.Utility;

#region using directives

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    internal const int RIBBON_WIDTH_I = 22,
        RIBBON_HORIZONTAL_OFFSET_I = -92;
    internal const float RIBBON_SCALE_F = 1.8f;

    #region textures

    public static Texture2D UltimateMeterTx { get; set; } =
        Game1.content.Load<Texture2D>(Path.Combine(ModEntry.Manifest.UniqueID, "UltimateMeter"));

    internal static Texture2D SkillBarTx { get; set; } =
        Game1.content.Load<Texture2D>(Path.Combine(ModEntry.Manifest.UniqueID, "SkillBars"));

    internal static Texture2D RibbonTx { get; set; } =
        Game1.content.Load<Texture2D>(Path.Combine(ModEntry.Manifest.UniqueID, "PrestigeRibbons"));

    internal static Texture2D MaxIconTx { get; set; } =
        Game1.content.Load<Texture2D>(Path.Combine(ModEntry.Manifest.UniqueID, "MaxFishSizeIcon"));

    #endregion textures
}