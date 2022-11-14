namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class SlingshotAssetRequestedEvent : AssetRequestedEvent
{
    private static readonly Dictionary<string, (Func<string> Provide, AssetLoadPriority Priority)> AssetProviders =
        new();

    /// <summary>Initializes a new instance of the <see cref="SlingshotAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        AssetProviders[$"{ModEntry.Manifest.UniqueID}/QuincyCollisionAnimation"] = (Provide: () => "assets/animations/quincy.png",
            Priority: AssetLoadPriority.Medium);
        AssetProviders[$"{ModEntry.Manifest.UniqueID}/SnowballCollisionAnimation"] = (Provide: () => "assets/animations/snowball.png",
            Priority: AssetLoadPriority.Medium);
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (AssetProviders.TryGetValue(e.NameWithoutLocale.Name, out var provider))
        {
            e.LoadFromModFile<Texture2D>(provider.Provide(), provider.Priority);
        }
    }
}
