namespace DaLion.Overhaul.Modules.Tweex.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal class TweexAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TweexAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TweexAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("Data/CraftingRecipes", new AssetEditor(EditCraftingRecipesData));
    }

    #region editor callbacks

    /// <summary>Edits crafting recipes with new ring recipes.</summary>
    private static void EditCraftingRecipesData(IAssetData asset)
    {
        if (!TweexModule.Config.ImmersiveGlowstoneProgression)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["Small Glow Ring"] = "336 2 768 5/Home/516/Ring/Mining 2";
        data["Small Magnet Ring"] = "335 2 769 5/Home/518/Ring/Mining 2";
        data["Glow Ring"] = "516 2 768 10/Home/517/Ring/Mining 4";
        data["Magnet Ring"] = "518 2 769 10/Home/519/Ring/Mining 4";
        data["Glowstone Ring"] = "517 1 519 1 768 20 769 20/Home/888/Ring/Mining 6";
    }

    #endregion editor callbacks
}
