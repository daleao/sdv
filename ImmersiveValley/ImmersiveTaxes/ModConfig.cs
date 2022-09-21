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
}
