namespace DaLion.Redux.Framework.Taxes;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for Taxes.</summary>
public sealed class Config
{
    /// <summary>
    ///     Gets the interest rate charged annually over any outstanding debt. Interest is accrued daily at a rate of 1/112 the
    ///     annual rate.
    /// </summary>
    [JsonProperty]
    public float AnnualInterest { get; internal set; } = 0.06f;

    /// <summary>Gets the taxable percentage of shipped products at the highest tax bracket.</summary>
    [JsonProperty]
    public float IncomeTaxCeiling { get; internal set; } = 0.37f;

    /// <summary>Gets a value indicating whether or not any gold spent on animal purchases and supplies is tax-deductible.</summary>
    [JsonProperty]
    public bool DeductibleAnimalExpenses { get; internal set; } = true;

    /// <summary>Gets a value indicating whether or not any gold spent constructing farm buildings is tax-deductible.</summary>
    [JsonProperty]
    public bool DeductibleBuildingExpenses { get; internal set; } = true;

    /// <summary>Gets a value indicating whether or not any gold spent on seed purchases is tax-deductible.</summary>
    [JsonProperty]
    public bool DeductibleSeedExpenses { get; internal set; } = true;

    /// <summary>Gets a value indicating whether or not any gold spent on tool purchases and upgrades is tax-deductible.</summary>
    [JsonProperty]
    public bool DeductibleToolExpenses { get; internal set; } = true;

    /// <summary>Gets a value indicating the list of extra objects that should be tax-deductible.</summary>
    [JsonProperty]
    public string[] DeductibleObjects { get; internal set; } = { };
}
