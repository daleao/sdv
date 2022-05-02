namespace DaLion.Stardew.Professions.Framework.Events.Content.AssetRequested;

#region using directives

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Extensions;
using GameLoop;
using Utility;

#endregion using directives

[UsedImplicitly]
internal class ProfessionAssetRequestedEvent : AssetRequestedEvent
{
    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/UltimateMeter"))
        {
            e.LoadFromModFile<Texture2D>("assets/hud/" +
                (Context.IsWorldReady &&
                ModEntry.ModHelper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP") &&
                !ModEntry.Config.DisableGaldoranTheme &&
                (Game1.currentLocation.NameOrUniqueName.IsAnyOf("Custom_CastleVillageOutpost", "Custom_CrimsonBadlands",
                    "Custom_IridiumQuarry", "Custom_TreasureCave") || ModEntry.Config.UseGaldoranThemeAllTimes)
                    ? "gauge_galdora.png"
                    : ModEntry.Config.UseVintageInterface
                        ? "gauge_vintage.png"
                        : "gauge.png"), AssetLoadPriority.Medium);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/SkillBars"))
        {
            e.LoadFromModFile<Texture2D>(
                "assets/menus/" + (ModEntry.Config.UseVintageInterface ? "skillbars_vintage.png" : "skillbars.png"),
                AssetLoadPriority.Medium);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/PrestigeRibbons"))
        {
            e.LoadFromModFile<Texture2D>("assets/sprites/ribbons.png", AssetLoadPriority.Medium);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/MaxFishSizeIcon"))
        {
            e.LoadFromModFile<Texture2D>("assets/menus/max.png", AssetLoadPriority.Medium);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/HudPointer"))
        {
            e.LoadFromModFile<Texture2D>("assets/hud/pointer.png", AssetLoadPriority.Medium);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/Spritesheet"))
        {
            e.LoadFrom(
                () => Textures.Spritesheet,
                AssetLoadPriority.Medium
            );
        }
    }
}