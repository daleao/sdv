namespace DaLion.Taxes;

/// <summary>The <see cref="TaxesMod"/> API implementation.</summary>
public class TaxesApi : ITaxesApi
{
    /// <inheritdoc />
    public (int Due, int Income, int Expenses, float Deductions, int Taxable) CalculateIncomeTax(Farmer? farmer = null)
    {
        return RevenueService.CalculateTaxes(farmer ?? Game1.player);
    }

    /// <inheritdoc />
    public int CalculatePropertyTax()
    {
        return CountyService.CalculateTaxes();
    }
}
