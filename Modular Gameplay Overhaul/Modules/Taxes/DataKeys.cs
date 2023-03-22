#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
namespace DaLion.Overhaul.Modules.Taxes;

/// <summary>Holds the string keys of mod data fields used by <see cref="OverhaulModule.Taxes"/>.</summary>
internal sealed class DataKeys
{
    // farmer
    internal const string SeasonIncome = "SeasonIncome";
    internal const string BusinessExpenses = "BusinessExpenses";
    internal const string PercentDeductions = "PercentDeductions";
    internal const string DebtOutstanding = "DebtOutstanding";

    // farm
    internal const string AgricultureValue = "AgricultureValue";
    internal const string LivestockValue = "LivestockValue";
    internal const string BuildingValue = "BuildingValue";
    internal const string UsedTiles = "UsedTiles";
    internal const string UsableTiles = "UsableTiles";
}
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
