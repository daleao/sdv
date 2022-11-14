namespace DaLion.Ligo.Modules.Core.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CoreAssetRequestedEvent : AssetRequestedEvent
{
    private static readonly Dictionary<string, (Func<string> Provide, AssetLoadPriority Priority)> AssetProviders =
        new();

    /// <summary>Initializes a new instance of the <see cref="CoreAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        AssetProviders[$"{ModEntry.Manifest.UniqueID}/StunAnimation"] = (Provide: () => "assets/animations/stun.png",
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
