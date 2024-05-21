namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="CoreAssetRequestedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CoreAssetRequestedEvent(EventManager? manager = null)
    : AssetRequestedEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    protected override void Initialize()
    {
        this.Edit("Data/CraftingRecipes", new AssetEditor(EditCraftingRecipesData));

        this.Provide(
            $"{Manifest.UniqueID}/BleedAnimation",
            new ModTextureProvider(() => "assets/sprites/bleed.png"));
        this.Provide(
            $"{Manifest.UniqueID}/SlowAnimation",
            new ModTextureProvider(() => "assets/sprites/slow.png"));
        this.Provide(
            $"{Manifest.UniqueID}/StunAnimation",
            new ModTextureProvider(() => "assets/sprites/stun.png"));
        this.Provide(
            $"{Manifest.UniqueID}/PoisonAnimation",
            new ModTextureProvider(() => "assets/sprites/poison.png"));
    }

    /// <summary>Edits crafting recipes with new ring recipes.</summary>
    private static void EditCraftingRecipesData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        data["Small Glow Ring"] = "336 2 768 5/Home/516/Ring/Mining 2";
        data["Small Magnet Ring"] = "335 2 769 5/Home/518/Ring/Mining 2";
        data["Glow Ring"] = "516 2 768 10/Home/517/Ring/Mining 4";
        data["Magnet Ring"] = "518 2 769 10/Home/519/Ring/Mining 4";
        data["Glowstone Ring"] = "517 1 519 1 768 20 769 20/Home/888/Ring/Mining 6";
    }
}
