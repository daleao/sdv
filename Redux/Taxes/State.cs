namespace DaLion.Redux.Taxes;

/// <summary>Holds the runtime state variables of the Taxes module.</summary>
internal sealed class State
{
    internal int LatestAmountDue { get; set; }

    internal int LatestDebit { get; set; }
}
