using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace TheLion.Stardew.Professions.Framework.Utility;

/// <summary>Holds common non-replacing texture assets.</summary>
public static class Textures
{
    public static Texture2D RibbonTx { get; } = ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "sprites", "ribbons.png"));

    public static Texture2D SkillBarTx { get; } = ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets",
        "menus", ModEntry.Config.UseVintageSkillBars ? "skillbars_vintage.png" : "skillbars.png"));

    public static Texture2D MaxIconTx { get; } =
        ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "menus", "max.png"));

    public static int RibbonWidth => 22;
    public static int RibbonHorizontalOffset => -99;
    public static float RibbonScale => 1.8f;

    public static int MaxIconWidth => 38;
    public static int MaxIconHeight => 18;
}