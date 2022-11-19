namespace DaLion.Ligo.Modules.Arsenal.Extensions;

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
internal static class ItemExtensions
{
    /// <summary>Determines whether the <paramref name="item"/> is a Hero Soul.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is Hero Soul, otherwise <see langword="false"/>.</returns>
    internal static bool IsHeroSoul(this Item item)
    {
        return Ligo.Integrations.DynamicGameAssetsApi!.GetDGAItemId(item) ==
               ModEntry.Manifest.UniqueID + "/Hero Soul";
    }
}
