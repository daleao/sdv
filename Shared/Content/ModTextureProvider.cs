namespace DaLion.Shared.Content;

#region using directives

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directivese

/// <summary>Generates a new instance of the <see cref="ModTextureProvider"/> record.</summary>
/// <param name="GetPath">A delegate which returns the relative path to the <see cref="Texture2D"/> inside the mod folder.</param>
/// <param name="Priority">The priority for an asset load when multiple apply for the same asset.</param>
public record ModTextureProvider(Func<string> GetPath, AssetLoadPriority Priority = AssetLoadPriority.Medium) : IAssetProvider
{
    /// <inheritdoc />
    public void Provide(AssetRequestedEventArgs e)
    {
        e.LoadFromModFile<Texture2D>(this.GetPath.Invoke(), this.Priority);
    }
}
