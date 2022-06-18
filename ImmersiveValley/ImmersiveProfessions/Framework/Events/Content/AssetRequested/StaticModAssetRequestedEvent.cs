namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Extensions;
using Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticModAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Construct an instance.</summary>
    internal StaticModAssetRequestedEvent()
    {
        Enable();
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/UltimateMeter"))
        {
            e.LoadFromModFile<Texture2D>("assets/hud/" + 
                (Context.IsWorldReady && ModEntry.SVEConfig is not null &&
                ModEntry.SVEConfig.Value<bool?>("DisableGaldoranTheme") == false &&
                (Game1.currentLocation.NameOrUniqueName.IsIn(
                "Custom_CastleVillageOutpost", "Custom_CrimsonBadlands",
                "Custom_IridiumQuarry", "Custom_TreasureCave") ||
                ModEntry.SVEConfig.Value<bool?>("UseGaldoranThemeAllTimes") == true)
                ? "gauge_galdora.png"
                : ModEntry.PlayerState.VintageInterface != "off"
                    ? $"gauge_vintage_{ModEntry.PlayerState.VintageInterface}.png"
                    : "gauge.png"), AssetLoadPriority.Medium);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/SkillBars"))
        {
            e.LoadFromModFile<Texture2D>(
                "assets/menus/" + (ModEntry.PlayerState.VintageInterface != "off" ? "skillbars_vintage.png" : "skillbars.png"),
                AssetLoadPriority.Medium);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/PrestigeProgression"))
        {
            e.LoadFromModFile<Texture2D>($"assets/sprites/{ModEntry.Config.PrestigeProgressionStyle}.png",
                AssetLoadPriority.Medium);
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