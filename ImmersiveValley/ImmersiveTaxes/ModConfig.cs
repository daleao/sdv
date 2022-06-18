namespace DaLion.Stardew.Taxes;

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    /// <summary>The taxable percentage of shipped products at the highest tax bracket.</summary>
    public float IncomeTaxCeiling = 0.37f;

    /// <summary>The interest rate charged annually over any outstanding debt. Interest is accrued daily at a rate of 1/112 the annual rate.</summary>
    public float AnnualInterest = 0.06f;
}