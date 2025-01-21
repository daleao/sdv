namespace DaLion.Professions.Framework.Chroma;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion using directives

internal static class ChromaMapper
{
    static ChromaMapper()
    {
        PopulateItems();
    }

    internal static IReadOnlyDictionary<string, Color> ColorFromTag { get; } = new Dictionary<string, Color>()
    {
        { "color_black", new Color(45, 45, 45) },
        { "color_gray", Color.Gray },
        { "color_white", Color.White },
        { "color_pink", new Color(255, 163, 186) },
        { "color_red", new Color(220, 0, 0) },
        { "color_orange", new Color(255, 128, 0) },
        { "color_yellow", new Color(255, 230, 0) },
        { "color_green", new Color(10, 143, 0) },
        { "color_blue", new Color(46, 85, 183) },
        { "color_purple", new Color(115, 41, 181) },
        { "color_brown", new Color(130, 73, 37) },
        { "color_light_cyan", new Color(180, 255, 255) },
        { "color_cyan", Color.Cyan },
        { "color_aquamarine", Color.Aquamarine },
        { "color_sea_green", Color.SeaGreen },
        { "color_lime", Color.Lime },
        { "color_yellow_green", Color.GreenYellow },
        { "color_pale_violet_red", Color.PaleVioletRed },
        { "color_salmon", new Color(255, 85, 95) },
        { "color_jade", new Color(130, 158, 93) },
        { "color_sand", Color.NavajoWhite },
        { "color_poppyseed", new Color(82, 47, 153) },
        { "color_dark_red", Color.DarkRed },
        { "color_dark_orange", Color.DarkOrange },
        { "color_dark_yellow", Color.DarkGoldenrod },
        { "color_dark_green", Color.DarkGreen },
        { "color_dark_blue", Color.DarkBlue },
        { "color_dark_purple", Color.DarkViolet },
        { "color_dark_pink", Color.DeepPink },
        { "color_dark_cyan", Color.DarkCyan },
        { "color_dark_gray", Color.DarkGray },
        { "color_dark_brown", Color.SaddleBrown },
        { "color_gold", Color.Gold },
        { "color_copper", new Color(179, 85, 0) },
        { "color_iron", new Color(197, 213, 224) },
        { "color_iridium", new Color(105, 15, 255) },
    };

    internal static Dictionary<Color, List<string>> ItemsByColor { get; } = [];

    private static void PopulateItems()
    {
        foreach (var color in ColorFromTag.Values)
        {
            ItemsByColor[color] = [];
        }

        foreach (var (key, data) in Game1.objectData)
        {
            if (data.ContextTags is null)
            {
                continue;
            }

            foreach (var tag in data.ContextTags)
            {
                if (tag.StartsWith("color_") && ColorFromTag.TryGetValue(tag, out var color))
                {
                    ItemsByColor[color].Add(key);
                }
            }
        }
    }
}
