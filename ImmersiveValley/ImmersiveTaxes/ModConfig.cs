namespace DaLion.Stardew.Taxes;

/// <summary>The mod user-defined settings.</summary>
public sealed class ModConfig
{
    /// <summary>
    ///     Gets or sets the interest rate charged annually over any outstanding debt. Interest is accrued daily at a rate of 1/112 the
    ///     annual rate.
    /// </summary>
    public float AnnualInterest { get; set; } = 0.06f;

    /// <summary>Gets or sets the taxable percentage of shipped products at the highest tax bracket.</summary>
    public float IncomeTaxCeiling { get; set; } = 0.37f;

    /// <summary>Gets or sets a value indicating whether or not any gold spent on animal purchases and supplies is tax-deductible.</summary>
    public bool DeductibleAnimalExpenses { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether or not any gold spent constructing farm buildings is tax-deductible.</summary>
    public bool DeductibleBuildingExpenses { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether or not any gold spent on seed purchases is tax-deductible.</summary>
    public bool DeductibleSeedExpenses { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether or not any gold spent on tool purchases and upgrades is tax-deductible.</summary>
    public bool DeductibleToolExpenses { get; set; } = true;

    /// <summary>Gets or sets a value indicating the list of extra objects that should be tax-deductible.</summary>
    public string[] DeductibleObjects { get; set; } = { };
}
