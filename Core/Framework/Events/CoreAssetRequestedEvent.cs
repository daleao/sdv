namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using StardewValley.GameData.Objects;

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
        this.Edit("Data/Objects", new AssetEditor(EditObjectsData));

        this.Provide(
            $"{Manifest.UniqueID}_BleedAnimation",
            new ModTextureProvider(() => "assets/sprites/bleed.png"));
        this.Provide(
            $"{Manifest.UniqueID}_SlowAnimation",
            new ModTextureProvider(() => "assets/sprites/slow.png"));
        this.Provide(
            $"{Manifest.UniqueID}_StunAnimation",
            new ModTextureProvider(() => "assets/sprites/stun.png"));
        this.Provide(
            $"{Manifest.UniqueID}_PoisonAnimation",
            new ModTextureProvider(() => "assets/sprites/poison.png"));
    }

    /// <summary>Makes seaweed an algae item.</summary>
    private static void EditObjectsData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, ObjectData>().Data;
        data["152"].ContextTags.Add("algae_item");
    }
}
