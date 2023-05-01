namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Shared.Content;
using DaLion.Shared.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HudPointerAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="HudPointerAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal HudPointerAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Provide(
            $"{Manifest.UniqueID}/HudPointer",
            new ModTextureProvider(() => "assets/hud/pointer.png"));
    }

    /// <inheritdoc />
    public override bool IsEnabled => HudPointer.Instance.IsValueCreated;
}
