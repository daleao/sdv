using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace TheLion.Stardew.Professions.Framework.Utility;

/// <summary>Holds static properties related to non-replacing texture assets.</summary>
public static class Textures
{
    internal static Texture2D SuperModeGaugeTx { get; set; } = ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "hud",
        ModEntry.Config.UseVintageSkillBars ? "bar_vintage.png" : "bar.png"));

    internal static Texture2D SkillBarTx { get; set; } = ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets",
        "menus", ModEntry.Config.UseVintageSkillBars ? "skillbars_vintage.png" : "skillbars.png"));

    internal static Texture2D RibbonTx { get; } =
        ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "sprites", "ribbons.png"));

    internal static Texture2D MaxIconTx { get; } =
        ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "menus", "max.png"));

    internal static int RibbonWidth => 22;
    internal static int RibbonHorizontalOffset => -99;
    internal static float RibbonScale => 1.8f;

    internal static int MaxIconWidth => 38;
    internal static int MaxIconHeight => 18;
}