namespace DaLion.Stardew.Alchemy.Framework.Events.Content;

#region using directives

using Microsoft.Xna.Framework.Graphics;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

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
        if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/Interface"))
            e.LoadFromModFile<Texture2D>("assets/interface.png", AssetLoadPriority.Medium);
        else if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/Objects"))
            e.LoadFromModFile<Texture2D>("assets/objects.png", AssetLoadPriority.Medium);
        else if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/Talents"))
            e.LoadFromModFile<Texture2D>("assets/talents.png", AssetLoadPriority.Medium);
        //else if (e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/Animations"))
        //    e.LoadFromModFile<Texture2D>("assets/animations.png", AssetLoadPriority.Medium);
    }
}