namespace DaLion.Professions;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

internal static class Textures
{
    internal const float STARS_SCALE = 3f;
    internal const int STARS_WIDTH = 20;
    internal const int PROGRESSION_HORIZONTAL_OFFSET = -104;
    internal const int PROGRESSION_VERTICAL_OFFSET = -86;

    private static Lazy<Texture2D> _prestigeRibbons =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{UniqueId}/PrestigeRibbons"));

    private static Lazy<Texture2D> _maxIcon =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{UniqueId}/MaxIcon"));

    private static Lazy<Texture2D> _skillBars =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{UniqueId}/SkillBars"));

    private static Lazy<Texture2D> _limitGauge =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{UniqueId}/LimitGauge"));

    internal static Texture2D PrestigeRibbons => _prestigeRibbons.Value;

    internal static Texture2D MaxIcon => _maxIcon.Value;

    internal static Texture2D SkillBars => _skillBars.Value;

    internal static Texture2D LimitGauge => _limitGauge.Value;

    internal static void Reload(IEnumerable<IAssetName> assets)
    {
        var names = assets.Select(a => a.BaseName).ToHashSet();
        if (names.Contains("PrestigeRibbons"))
        {
            _prestigeRibbons = new Lazy<Texture2D>(() =>
                ModHelper.GameContent.Load<Texture2D>($"{UniqueId}/PrestigeRibbons"));
        }

        if (names.Contains("MaxIcon"))
        {
            _maxIcon = new Lazy<Texture2D>(() =>
                ModHelper.GameContent.Load<Texture2D>($"{UniqueId}/MaxIcon"));
        }

        if (names.Contains("SkillBars"))
        {
            _skillBars = new Lazy<Texture2D>(() =>
                ModHelper.GameContent.Load<Texture2D>($"{UniqueId}/SkillBars"));
        }

        if (names.Contains("LimitGauge"))
        {
            _limitGauge = new Lazy<Texture2D>(() =>
                ModHelper.GameContent.Load<Texture2D>($"{UniqueId}/LimitGauge"));
        }
    }
}
