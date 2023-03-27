namespace DaLion.Overhaul.Modules.Taxes.Extensions;

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    /// <summary>Determines whether the <paramref name="farmer"/> is liable for paying taxes.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> is the main player or using separate wallets, otherwise <see langword="false"/>.</returns>
    internal static bool ShouldPayTaxes(this Farmer farmer)
    {
        return Context.IsMainPlayer || farmer.useSeparateWallets;
    }
}
