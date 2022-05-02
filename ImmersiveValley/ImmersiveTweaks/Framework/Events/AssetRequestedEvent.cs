namespace DaLion.Stardew.Tweaks.Framework.Events;

#region using directives

using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.AssetRequested"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal class AssetRequestedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.Content.AssetRequested += OnAssetRequested;
        Log.D("[Tweaks] Hooked AssetRequested event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.Content.AssetRequested -= OnAssetRequested;
        Log.D("[Tweaks] Unhooked AssetRequested event.");
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/BetterHoneyMeadIcons")) return;

        e.LoadFromModFile<Texture2D>($"assets/mead-{ModEntry.Config.HoneyMeadStyle.ToLower()}.png",
            AssetLoadPriority.Medium);
    }
}