namespace DaLion.Shared.Extensions.SMAPI;

#region using directives

using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="IAssetName"/> interface.</summary>
public static class AssetNameExtensions
{
    /// <summary>Determines whether the given asset name is equivalent to any of the specified <paramref name="candidates"/>.</summary>
    /// <param name="name">The <see cref="IAssetName"/>.</param>
    /// <param name="candidates">An array of candidate <see cref="string"/>s.</param>
    /// <returns><see langword="true"/> if the asset name is equivalent to at least one of the <paramref name="candidates"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsEquivalentToAnyOf(this IAssetName name, params string[] candidates)
    {
        return candidates.Any(c => name.IsEquivalentTo(c));
    }
}
