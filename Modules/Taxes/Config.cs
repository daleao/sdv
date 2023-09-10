namespace DaLion.Overhaul.Modules.Taxes;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for TXS.</summary>
public sealed class Config : Shared.Configs.Config
{
    /// <summary>Gets the income thresholds that determine each tax bracket.</summary>
    [JsonProperty]
    public int[] IncomeBrackets { get; internal set; } = { 9950, 40525, 86375, 164925, 209425, 523600 };

    /// <summary>Gets the taxable percentage of income at each bracket. If there are n brackets, this array should contain n+1 elements.</summary>
    [JsonProperty]
    public float[] TaxPerBracket { get; internal set; } = { 0.1f, 0.12f, 0.22f, 0.24f, 0.32f, 0.35f, 0.37f };

    /// <summary>
    ///     Gets the interest rate charged annually over any outstanding debt. Interest is accrued daily at a rate of 1/112 the
    ///     annual rate.
    /// </summary>
    [JsonProperty]
    public float AnnualInterest { get; internal set; } = 0.12f;

    /// <summary>Gets the flat rate charged over due income taxes not paid in time.</summary>
    [JsonProperty]
    public float IncomeTaxLatenessFine { get; internal set; } = 0.05f;

    /// <summary>Gets a value indicating whether or not any gold spent on animal purchases and supplies should be tax-deductible.</summary>
    [JsonProperty]
    public float DeductibleAnimalExpenses { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether or not any gold spent constructing farm buildings should be tax-deductible.</summary>
    [JsonProperty]
    public float DeductibleBuildingExpenses { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether or not any gold spent on seed purchases should be tax-deductible.</summary>
    [JsonProperty]
    public float DeductibleSeedExpenses { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether or not any gold spent on tool purchases and upgrades should be tax-deductible.</summary>
    [JsonProperty]
    public float DeductibleToolExpenses { get; internal set; } = 1f;

    /// <summary>Gets a value indicating the list of extra objects that should be tax-deductible.</summary>
    [JsonProperty]
    public string[] DeductibleExtras { get; internal set; } = Array.Empty<string>();

    /// <summary>Gets the flat rate charged over due income taxes not paid in time.</summary>
    [JsonProperty]
    public float PropertyTaxLatenessFine { get; internal set; } = 0.15f;

    /// <summary>Gets the property tax rate of an unused tile.</summary>
    [JsonProperty]
    public float UnusedTileTaxRate { get; internal set; } = 0.05f;

    /// <summary>Gets the property tax rate of a tile used for agriculture or livestock.</summary>
    [JsonProperty]
    public float UsedTileTaxRate { get; internal set; } = 0.02f;

    /// <summary>Gets the property tax rate of a tile used for real-estate.</summary>
    [JsonProperty]
    public float BuildingTaxRate { get; internal set; } = 0.04f;

    /// <summary>Gets a value indicating whether or not magical buildings are exempted from property taxes.</summary>
    [JsonProperty]
    public bool ExemptMagicalBuilding { get; internal set; } = true;

    /// <inheritdoc />
    public override bool Validate()
    {
        Log.T("[TXS]: Verifying tax configs...");

        if (this.IncomeBrackets.Length == this.TaxPerBracket.Length - 1)
        {
            return true;
        }

        Log.W("[TXS]: Mismatch between number of income brackets and tax values." +
              " For `n` income brackets there should be `n+1` tax values (the final value implicitly corresponds to infinity)." +
              " The default values will be restored.");
        this.IncomeBrackets = new[] { 9950, 40525, 86375, 164925, 209425, 523600 };
        this.TaxPerBracket = new[] { 0.1f, 0.12f, 0.22f, 0.24f, 0.32f, 0.35f, 0.37f };
        return false;
    }
}
