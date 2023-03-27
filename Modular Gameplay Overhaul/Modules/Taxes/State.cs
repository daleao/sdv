namespace DaLion.Overhaul.Modules.Taxes;

/// <summary>The runtime state for Tax variables.</summary>
internal sealed class State
{
    internal int LatestDueIncomeTax { get; set; }

    internal int LatestOutstandingIncomeTax { get; set; }

    internal float LatestTaxDeductions { get; set; }

    internal int LatestDuePropertyTax { get; set; }

    internal int LatestOutstandingPropertyTax { get; set; }

    internal int LatestAmountWithheld { get; set; }
}
