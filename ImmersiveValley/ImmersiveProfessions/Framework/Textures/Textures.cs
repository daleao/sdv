namespace DaLion.Stardew.Professions.Framework.Textures;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
public static class Textures
{
    internal const float RibbonScale = 1.8f;
    internal const float StarsScale = 3f;

    internal const int RibbonWidth = 22;
    internal const int StarsWidth = 20;
    internal const int SingleStarWidth = 8;
    internal const int ProgressionHorizontalOffset = -82;
    internal const int ProgressionVerticalOffset = -70;

    /// <summary>Gets the <see cref="Texture2D"/> used by <see cref="HudPointer"/>.</summary>
    public static Texture2D PointerTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/HudPointer");

    /// <summary>Gets the <see cref="Texture2D"/> which indicates a fish caught at max size in the <see cref="StardewValley.Menus.CollectionsPage"/>.</summary>
    public static Texture2D MaxIconTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/MaxFishSizeIcon");

    /// <summary>Gets the <see cref="Texture2D"/> which contains most of the sprites used by this mod.</summary>
    public static Texture2D SpriteTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/SpriteSheet");

    /// <summary>Gets the <see cref="Texture2D"/> used to display skill reset progression.</summary>
    public static Texture2D ProgressionTx { get; private set; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/PrestigeProgression");

    /// <summary>Gets the <see cref="Texture2D"/> used to draw skill levels in the <see cref="StardewValley.Menus.SkillsPage"/>.</summary>
    public static Texture2D BarsTx { get; private set; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/SkillBars");

    /// <summary>Gets the <see cref="Texture2D"/> used to draw the <see cref="Ultimates.UltimateHud"/>.</summary>
    public static Texture2D MeterTx { get; private set; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/UltimateMeter");

    internal static void Refresh(IReadOnlySet<IAssetName> names)
    {
        if (names.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/SkillBars")))
        {
            BarsTx = ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/SkillBars");
        }

        if (names.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/UltimateMeter")))
        {
            MeterTx = ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/UltimateMeter");
        }

        if (names.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/PrestigeProgression")))
        {
            ProgressionTx = ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/PrestigeProgression");
        }
    }
}
