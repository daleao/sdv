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
    public (int AgricultureValue, int LivestockValue, int BuildingValue, int UsedTiles) CalculatePropertyTax()
    {
        return Game1.getFarm().Appraise(false);
    }
}
