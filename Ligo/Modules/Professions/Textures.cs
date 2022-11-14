namespace DaLion.Ligo.Modules.Professions;

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

    internal static Texture2D PointerTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/HudPointer");

    internal static Texture2D MaxIconTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/MaxIcon");

    internal static Texture2D SkillBarsTx { get; private set; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/SkillBars");

    internal static Texture2D UltimateMeterTx { get; private set; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/UltimateMeter");

    internal static Texture2D BuffsSheetTx { get; } =
        ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/sprites/buffs");

    internal static Texture2D ProfessionsSheetTx { get; } =
        ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/sprites/professions");

    internal static Texture2D PrestigeSheetTx { get; private set; } =
        ModEntry.ModHelper.ModContent.Load<Texture2D>($"assets/sprites/{ModEntry.Config.Professions.PrestigeProgressionStyle}.png");

    internal static void Refresh(IReadOnlySet<IAssetName> names)
    {
        if (names.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/SkillBars")))
        {
            SkillBarsTx = ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/SkillBars");
        }

        if (names.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/UltimateMeter")))
        {
            UltimateMeterTx = ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/UltimateMeter");
        }

        if (names.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/PrestigeProgression")))
        {
            PrestigeSheetTx = ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/PrestigeProgression");
        }
    }
}
