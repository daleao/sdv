namespace DaLion.Ligo.Modules.Core.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

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
            $"{ModEntry.Manifest.UniqueID}/StunAnimation",
            new ModTextureProvider(() => "assets/animations/stun.png", AssetLoadPriority.Medium));
    }
}
