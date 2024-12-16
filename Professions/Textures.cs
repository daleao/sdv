namespace DaLion.Professions;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

internal static class Textures
{
    internal const float STARS_SCALE = 3f;
    internal const int STARS_WIDTH = 20;
    internal const int PROGRESSION_HORIZONTAL_OFFSET = -104;
    internal const int PROGRESSION_VERTICAL_OFFSET = -74;

    private static Lazy<Texture2D> _prestigeRibbons =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{UniqueId}_PrestigeRibbons"));

    private static Lazy<Texture2D> _maxIcon =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{UniqueId}_MaxIcon"));

    private static Lazy<Texture2D> _skillBars =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{UniqueId}_SkillBars"));

    private static Lazy<Texture2D> _limitGauge =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{UniqueId}_LimitGauge"));

    private static Lazy<Texture2D> _masteredSkillIcons =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{UniqueId}_MasteredSkillIcons"));

    internal static Texture2D PrestigeRibbons => _prestigeRibbons.Value;

    internal static Texture2D MaxIcon => _maxIcon.Value;

    internal static Texture2D SkillBars => _skillBars.Value;

    internal static Texture2D LimitGauge => _limitGauge.Value;

    internal static Texture2D MasteredSkillIcons => _masteredSkillIcons.Value;

    internal static void Reload(IEnumerable<IAssetName> assets)
    {
        var names = assets.Select(a => a.BaseName).ToHashSet();
        if (names.Contains("PrestigeRibbons"))
        {
            _prestigeRibbons = new Lazy<Texture2D>(() =>
                ModHelper.GameContent.Load<Texture2D>($"{UniqueId}_PrestigeRibbons"));
        }

        if (names.Contains("MaxIcon"))
        {
            _maxIcon = new Lazy<Texture2D>(() =>
                ModHelper.GameContent.Load<Texture2D>($"{UniqueId}_MaxIcon"));
        }

        if (names.Contains("SkillBars"))
        {
            _skillBars = new Lazy<Texture2D>(() =>
                ModHelper.GameContent.Load<Texture2D>($"{UniqueId}_SkillBars"));
        }

        if (names.Contains("LimitGauge"))
        {
            _limitGauge = new Lazy<Texture2D>(() =>
                ModHelper.GameContent.Load<Texture2D>($"{UniqueId}_LimitGauge"));
        }

        if (names.Contains("MasteredSkillIcons"))
        {
            _masteredSkillIcons = new Lazy<Texture2D>(() =>
                ModHelper.GameContent.Load<Texture2D>($"{UniqueId}_MasteredSkillIcons"));
        }
    }
}
