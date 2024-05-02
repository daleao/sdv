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
    private readonly Dictionary<string, float> _deductibleExtras = new()
    {
        { "Example Object and Percentage", 1f },
    };

    private float _incomeLatenessFine = 0.05f;
    private float _deductibleAnimalExpenses = 1f;
    private float _deductibleBuildingExpenses = 1f;
    private float _deductibleSeedExpenses = 1f;
    private float _deductibleToolExpenses = 1f;
    private float _propertyLatenessFine = 0.15f;
    private float _unusedTileTaxRate = 0.05f;
    private float _usedTileTaxRate = 0.02f;
    private float _buildingTaxRate = 0.04f;
    private float _annualInterest = 0.72f;
    private Dictionary<int, float> _taxRatePerIncomeBracket = new()
    {
        { 9950, 0.1f },
        { 40525, 0.12f },
        { 86375, 0.22f },
        { 164925, 0.24f },
        { 209425, 0.32f },
        { 523600, 0.35f },
        { int.MaxValue, 0.37f },
    };

    #region income

    /// <summary>Gets the taxable income percentage at each income threshold.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(0)]
    [GMCMOverride(typeof(TaxesConfigMenu), "TaxByIncomeBracketOverride")]
    public Dictionary<int, float> TaxRatePerIncomeBracket
    {
        get => this._taxRatePerIncomeBracket;
        internal set
        {
            if (value == this._taxRatePerIncomeBracket)
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

            this._taxRatePerIncomeBracket = value;
            if (Context.IsWorldReady)
            {
                RevenueService.TaxByIncomeBracket = value.ToImmutableDictionary();
            }
        }
    }

    /// <summary>Gets the percentage rate charged over due income taxes not paid in time.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(1)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.01f)]
    public float IncomeTaxLatenessFine
    {
        get => this._incomeLatenessFine;
        internal set
        {
            this._incomeLatenessFine = Math.Max(value, 0f);
        }
    }

    /// <summary>Gets the percentage of gold spent on animal purchases and supplies that should be tax-deductible.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(2)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.01f)]
    public float DeductibleAnimalExpenses
    {
        get => this._deductibleAnimalExpenses;
        internal set
        {
            this._deductibleAnimalExpenses = Math.Clamp(value, 0f, 1f);
        }
    }

    /// <summary>Gets the percentage of gold spent constructing farm buildings that should be tax-deductible.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(3)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.01f)]
    public float DeductibleBuildingExpenses
    {
        get => this._deductibleBuildingExpenses;
        internal set
        {
            this._deductibleBuildingExpenses = Math.Clamp(value, 0f, 1f);
        }
    }

    /// <summary>Gets the percentage of gold spent on seed purchases that should be tax-deductible.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(4)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.01f)]
    public float DeductibleSeedExpenses
    {
        get => this._deductibleSeedExpenses;
        internal set
        {
            this._deductibleSeedExpenses = Math.Clamp(value, 0f, 1f);
        }
    }

    /// <summary>Gets the percentage of gold spent on tool purchases and upgrades that should be tax-deductible.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(5)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.01f)]
    public float DeductibleToolExpenses
    {
        get => this._deductibleToolExpenses;
        internal set
        {
            this._deductibleToolExpenses = Math.Clamp(value, 0f, 1f);
        }
    }

    /// <summary>Gets a dictionary of extra objects that should be tax-deductible.</summary>
    [JsonProperty]
    [GMCMSection("txs.income")]
    [GMCMPriority(6)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.01f)]
    [GMCMOverride(typeof(TaxesConfigMenu), "DeductibleExtrasOverride")]
    [GMCMIgnore]
    public Dictionary<string, float> DeductibleExtras
    {
        get => this._deductibleExtras;
        internal set
        {
            foreach (var pair in value)
            {
                this._deductibleExtras[pair.Key] = Math.Clamp(pair.Value, 0f, 1f);
            }
        }
    }

    #endregion income

    #region property

    /// <summary>Gets the flat rate charged over due income taxes not paid in time.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(10)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.01f)]
    public float PropertyTaxLatenessFine
    {
        get => this._propertyLatenessFine;
        internal set
        {
            this._propertyLatenessFine = Math.Max(value, 0f);
        }
    }

    /// <summary>Gets the property tax rate of an unused tile.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(11)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.01f)]
    public float UnusedTileTaxRate
    {
        get => this._unusedTileTaxRate;
        internal set
        {
            this._unusedTileTaxRate = Math.Max(value, 0f);
        }
    }

    /// <summary>Gets the property tax rate of a tile used for agriculture or livestock.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(12)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.01f)]
    public float UsedTileTaxRate
    {
        get => this._usedTileTaxRate;
        internal set
        {
            this._usedTileTaxRate = Math.Max(value, 0f);
        }
    }

    /// <summary>Gets the property tax rate of a tile used for real-estate.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(13)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.01f)]
    public float BuildingTaxRate
    {
        get => this._buildingTaxRate;
        internal set
        {
            this._buildingTaxRate = Math.Max(value, 0f);
        }
    }

    /// <summary>Gets a value indicating whether magical buildings are exempted from property taxes.</summary>
    [JsonProperty]
    [GMCMSection("txs.property")]
    [GMCMPriority(14)]
    public bool ExemptMagicalBuildings { get; internal set; } = true;

    #endregion property

    #region other

    /// <summary>
    ///     Gets the interest rate charged annually over any outstanding debt. Interest is accrued daily at a rate of 1/112 the
    ///     annual rate.
    /// </summary>
    [JsonProperty]
    [GMCMSection("other")]
    [GMCMPriority(50)]
    [GMCMRange(0f, 2f)]
    [GMCMInterval(0.01f)]
    public float AnnualInterest
    {
        get => this._annualInterest;
        internal set
        {
            this._annualInterest = Math.Max(value, 0f);
        }
    }

    #endregion other
}
