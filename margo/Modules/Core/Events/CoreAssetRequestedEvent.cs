namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CoreAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CoreAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Provide(
            $"{Manifest.UniqueID}/BleedAnimation",
            new ModTextureProvider(() => "assets/sprites/effects/bleed.png"));
        this.Provide(
            $"{Manifest.UniqueID}/SlowAnimation",
            new ModTextureProvider(() => "assets/sprites/effects/slow.png"));
        this.Provide(
            $"{Manifest.UniqueID}/StunAnimation",
            new ModTextureProvider(() => "assets/sprites/effects/stun.png"));
        this.Provide(
            $"{Manifest.UniqueID}/PoisonAnimation",
            new ModTextureProvider(() => "assets/sprites/effects/poison.png"));
    }
}
