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

    internal static Texture2D MaxIconTx { get; } =
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/MaxIcon");

    internal static Texture2D SkillBarsTx { get; private set; } =
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/SkillBars");

    internal static Texture2D UltimateMeterTx { get; private set; } =
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/UltimateMeter");

    internal static Texture2D PrestigeSheetTx { get; private set; } =
        ModHelper.ModContent.Load<Texture2D>($"assets/sprites/{ProfessionsModule.Config.ProgressionStyle}.png");

    internal static void Refresh(IReadOnlySet<IAssetName> names)
    {
        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/SkillBars")))
        {
            SkillBarsTx = ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/SkillBars");
        }

        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/UltimateMeter")))
        {
            UltimateMeterTx = ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/UltimateMeter");
        }

        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/PrestigeProgression")))
        {
            PrestigeSheetTx = ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/PrestigeProgression");
        }
    }
}
