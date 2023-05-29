namespace DaLion.Overhaul;

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

    internal static Texture2D GemstonesTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/sprites/gemstones");

    internal static Texture2D RingsTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/sprites/rings");

    internal static Texture2D PatternedResonanceTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/vfx/resonance_patterned");

    internal static Texture2D StrongerResonanceTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/vfx/resonance_stronger");

    internal static Texture2D ShieldTx { get; set; } =
        ModHelper.ModContent.Load<Texture2D>("assets/vfx/shield.png");

    internal static Texture2D GemSocketTx { get; set; } =
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/GemstoneSockets");

    internal static Texture2D PrestigeSheetTx { get; private set; } =
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/PrestigeProgression");

    internal static Texture2D MaxIconTx { get; } =
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/MaxIcon");

    internal static Texture2D SkillBarsTx { get; private set; } =
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/SkillBars");

    internal static Texture2D UltimateMeterTx { get; private set; } =
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/UltimateMeter");

    internal static void Refresh(IReadOnlySet<IAssetName> names)
    {
        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/GemstoneSockets")))
        {
            GemSocketTx = ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/GemstoneSockets");
        }

        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/PrestigeProgression")))
        {
            PrestigeSheetTx = ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/PrestigeProgression");
        }

        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/SkillBars")))
        {
            SkillBarsTx = ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/SkillBars");
        }

        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/UltimateMeter")))
        {
            UltimateMeterTx = ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/UltimateMeter");
        }
    }
}
