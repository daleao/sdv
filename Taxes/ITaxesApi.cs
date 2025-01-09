namespace DaLion.Taxes;

/// <summary>The public interface for the Taxes mod API.</summary>
public interface ITaxesApi
{
    /// <summary>Evaluates the due income tax and other relevant stats for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>. Defaults to <see cref="Game1.player"/>.</param>
    /// <returns>The amount of income tax due in gold, along with total income, business expenses, eligible deductions and total taxable amount (in that order).</returns>
    (int Due, int Income, int Expenses, float Deductions, int Taxable) CalculateIncomeTax(Farmer? farmer = null);

    /// <summary>Determines the total property value of the farm.</summary>
    /// <returns>The total values of agriculture activities, livestock and buildings on the farm, as well as the total number of tiles used by all of those activities.</returns>
    int CalculatePropertyTax();
}
