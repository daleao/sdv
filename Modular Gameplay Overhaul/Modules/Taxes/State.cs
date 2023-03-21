namespace DaLion.Overhaul.Modules.Taxes;

/// <summary>The ephemeral runtime state for Taxes.</summary>
internal sealed class State
{
    internal int LatestAmountDue { get; set; }

    internal int LatestAmountWithheld { get; set; }

    internal int UsableFarmTileCount { get; set; } = -1;
}
