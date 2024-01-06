namespace DaLion.Overhaul.Modules.Professions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    internal const float RibbonScale = 1.8f;
    internal const float StarsScale = 3f;

    internal const int RibbonWidth = 22;
    internal const int StarsWidth = 20;
    internal const int SingleStarWidth = 8;
    internal const int ProgressionHorizontalOffset = -82;
    internal const int ProgressionVerticalOffset = -70;

    private static Lazy<Texture2D> _prestigeSheetTx =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/PrestigeRibbons"));

    private static Lazy<Texture2D> _maxIconTx =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/MaxIcon"));

    private static Lazy<Texture2D> _skillBarsTx =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/SkillBars"));

    private static Lazy<Texture2D> _ultimateMeterTx =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/UltimateMeter"));

    internal static Texture2D PrestigeSheetTx => _prestigeSheetTx.Value;

    internal static Texture2D MaxIconTx => _maxIconTx.Value;

    internal static Texture2D SkillBarsTx => _skillBarsTx.Value;

    internal static Texture2D UltimateMeterTx => _ultimateMeterTx.Value;

    internal static void Refresh(IReadOnlySet<IAssetName> names)
    {
        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/PrestigeRibbons")))
        {
            _prestigeSheetTx = new Lazy<Texture2D>(() => ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/PrestigeRibbons"));
        }

        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/SkillBars")))
        {
            _skillBarsTx = new Lazy<Texture2D>(() => ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/SkillBars"));
        }

        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/UltimateMeter")))
        {
            _ultimateMeterTx = new Lazy<Texture2D>(() => ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/UltimateMeter"));
        }
    }
}
