namespace DaLion.Taxes;

#region using directives

using System.Collections.Generic;
using System.Collections.Immutable;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Config schema for the Taxes mod.</summary>
public sealed class TaxesConfig
{
    #region income

    /// <summary>Gets the taxable income percentage at each income threshold.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(0)]
    [GMCMOverride(typeof(TaxesConfigMenu), "TaxByIncomeBracketOverride")]
    public Dictionary<int, float> TaxRatePerIncomeBracket
    {
        get;
        internal set
        {
            if (value == field)
            {
                return;
            }

            var previous = (0, 0f);
            foreach (var pair in value)
            {
                if (pair.Key <= 0 || pair.Key <= previous.Item1 || pair.Value is <= 0f or >= 1f ||
                    pair.Value < previous.Item2)
                {
                    return;
                }

                previous = (pair.Key, pair.Value);
            }

            field = value;
            if (Context.IsWorldReady)
            {
                RevenueService.TaxByIncomeBracket = value.ToImmutableDictionary();
            }
        }
    } = new()
    {
        { 9950, 0.1f },
        { 40525, 0.12f },
        { 86375, 0.22f },
        { 164925, 0.24f },
        { 209425, 0.32f },
        { 523600, 0.35f },
        { int.MaxValue, 0.37f },
    };

    /// <summary>Gets the percentage of gold spent on animal purchases and supplies that should be tax-deductible.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(1)]
    [GMCMRange(0f, 1f, 0.01f)]
    public float DeductibleAnimalExpenses
    {
        get;
        internal set
        {
            field = Math.Clamp(value, 0f, 1f);
        }
    } = 1f;

    /// <summary>Gets the percentage of gold spent constructing farm buildings that should be tax-deductible.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(2)]
    [GMCMRange(0f, 1f, 0.01f)]
    public float DeductibleBuildingExpenses
    {
        get;
        internal set
        {
            field = Math.Clamp(value, 0f, 1f);
        }
    } = 1f;

    /// <summary>Gets the percentage of gold spent on seed purchases that should be tax-deductible.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(3)]
    [GMCMRange(0f, 1f, 0.01f)]
    public float DeductibleSeedExpenses
    {
        get;
        internal set
        {
            field = Math.Clamp(value, 0f, 1f);
        }
    } = 1f;

    /// <summary>Gets the percentage of gold spent on tool purchases and upgrades that should be tax-deductible.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(4)]
    [GMCMRange(0f, 1f, 0.01f)]
    public float DeductibleToolExpenses
    {
        get;
        internal set
        {
            field = Math.Clamp(value, 0f, 1f);
        }
    } = 1f;

    /// <summary>Gets a dictionary of extra objects that should be tax-deductible.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(5)]
    [GMCMRange(0f, 1f, 0.01f)]
    [GMCMOverride(typeof(TaxesConfigMenu), "DeductibleExtrasOverride")]
    [GMCMIgnore]
    public Dictionary<string, float> DeductibleExtras
    {
        get;
        internal set
        {
            foreach (var pair in value)
            {
                field[pair.Key] = Math.Clamp(pair.Value, 0f, 1f);
            }
        }
    } = new() { { "Example Object and Percentage", 1f }, };

    /// <summary>Gets or sets the day of the season when income taxes are charged.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(6)]
    [GMCMRange(1, 27)]
    public int IncomeTaxDay { get; set; } = 5;

    /// <summary>Gets the percentage rate charged overdue income taxes not paid in time.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(7)]
    [GMCMRange(0f, 1f, 0.01f)]
    public float IncomeTaxLatenessFine
    {
        get;
        internal set
        {
            field = Math.Max(value, 0f);
        }
    } = 0.05f;

    #endregion income

    #region property

    /// <summary>Gets the baseline cost of each unused tile.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(10)]
    [GMCMRange(0, 100, 5)]
    public int BaselineUnusedTileCost
    {
        get;
        internal set
        {
            field = Math.Max(value, 0);
        }
    } = 15;

    /// <summary>Gets the property tax rate of an unused tile.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(11)]
    [GMCMRange(0f, 1f, 0.01f)]
    public float UnusedTileTaxRate
    {
        get;
        internal set
        {
            field = Math.Max(value, 0f);
        }
    } = 0.05f;

    /// <summary>Gets the property tax rate of a tile used for agriculture or livestock.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(12)]
    [GMCMRange(0f, 1f, 0.01f)]
    public float UsedTileTaxRate
    {
        get;
        internal set
        {
            field = Math.Max(value, 0f);
        }
    } = 0.02f;

    /// <summary>Gets the property tax rate of a tile used for real-estate.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(13)]
    [GMCMRange(0f, 1f, 0.01f)]
    public float BuildingTaxRate
    {
        get;
        internal set
        {
            field = Math.Max(value, 0f);
        }
    } = 0.1f;

    /// <summary>Gets a value indicating whether magical buildings are exempted from property taxes.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(14)]
    public bool ExemptMagicalBuildings { get; internal set; } = true;

    /// <summary>Gets or sets the day of the season when property taxes are charged.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(15)]
    [GMCMRange(1, 28)]
    public int PropertyTaxDay { get; set; } = 20;

    /// <summary>Gets the flat rate charged overdue income taxes not paid in time.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(16)]
    [GMCMRange(0f, 1f, 0.01f)]
    public float PropertyTaxLatenessFine
    {
        get;
        internal set
        {
            field = Math.Max(value, 0f);
        }
    } = 0.15f;

    #endregion property

    #region other

    /// <summary>
    ///     Gets the interest rate charged annually over any outstanding debt. Interest is accrued daily at a rate of 1/112 the
    ///     annual rate.
    /// </summary>
    [JsonProperty]
    [GMCMSection("other")]
    [GMCMPriority(50)]
    [GMCMRange(0f, 2f, 0.01f)]
    public float AnnualInterest
    {
        get;
        internal set
        {
            field = Math.Max(value, 0f);
        }
    } = 0.72f;

    /// <summary>Gets a multiplier which is used to estimate the value of artisan products derived from crops.</summary>
    [JsonProperty]
    [GMCMSection("other")]
    [GMCMPriority(51)]
    [GMCMRange(1f, 3f, 0.01f)]
    public float DefaultArtisanValueCropMultiplier
    {
        get;
        internal set
        {
            field = Math.Max(value, 0f);
        }
    } = 2f;

    /// <summary>Gets a multiplier which is used to estimate the value of artisan products derived from animal produce.</summary>
    [JsonProperty]
    [GMCMSection("other")]
    [GMCMPriority(52)]
    [GMCMRange(1f, 3f, 0.01f)]
    public float DefaultArtisanValueProduceMultiplier
    {
        get;
        internal set
        {
            field = Math.Max(value, 0f);
        }
    } = 1.5f;

    #endregion other
}
