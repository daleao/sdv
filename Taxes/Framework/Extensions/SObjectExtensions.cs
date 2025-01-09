namespace DaLion.Taxes.Framework.Extensions;

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Determines whether <paramref name="object"/> is an artisan machine.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a machine that creates artisan goods, otherwise <see langword="false"/>.</returns>
    internal static bool IsArtisanMachine(this SObject @object)
    {
        return Lookups.ArtisanMachines.Contains(@object.QualifiedItemId);
    }
}
