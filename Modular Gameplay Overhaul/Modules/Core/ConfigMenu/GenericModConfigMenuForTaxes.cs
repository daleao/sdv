namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

using System.Linq;

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the Taxes menu.</summary>
    private void RegisterTaxes()
    {
        this
            .AddPage(OverhaulModule.Taxes.Namespace, () => "Tax Settings")

            .AddSectionTitle(() => "Income Tax")
            .AddTextbox(
                () => "Income Brackets",
                () =>
                    "The income thresholds that determine each tax bracket. You can set a single very high value to disable income taxes entirely.",
                config => string.Join(", ", config.Taxes.IncomeBrackets),
                (config, value) => config.Taxes.IncomeBrackets =
                    value.Split(new[] { ", " }, StringSplitOptions.None).Select(int.Parse).ToArray())
            .AddTextbox(
                () => "Tax Per Brackets",
                () =>
                    "The taxable percentage of income at (up to) each bracket. If there are n brackets, this array should contain n+1 values. You can set all values to zero to disable income taxes entirely.",
                config => string.Join(", ", config.Taxes.IncomeTaxPerBracket),
                (config, value) => config.Taxes.IncomeTaxPerBracket =
                    value.Split(new[] { ", " }, StringSplitOptions.None).Select(float.Parse).ToArray())
            .AddNumberField(
                () => "Annual Interest",
                () =>
                    "The interest rate charged annually over any outstanding debt. Interest is accrued daily at a rate of 1/112 the annual rate.",
                config => config.Taxes.AnnualInterest,
                (config, value) => config.Taxes.AnnualInterest = value,
                0f,
                1f,
                0.02f)
            .AddNumberField(
                () => "Lateness Fine",
                () => "A flat rate, charged once over the unpaid amount, when income taxes are not paid on-time.",
                config => config.Taxes.IncomeTaxLatenessFine,
                (config, value) => config.Taxes.IncomeTaxLatenessFine = value,
                0f,
                1f,
                0.05f)
            .AddCheckbox(
                () => "Deductible Animal Expenses",
                () => "Whether or not any gold spent on animal purchases and supplies should be tax-deductible.",
                config => config.Taxes.DeductibleAnimalExpenses,
                (config, value) => config.Taxes.DeductibleAnimalExpenses = value)
            .AddCheckbox(
                () => "Deductible Building Expenses",
                () => "Whether or not any gold spent constructing farm buildings is tax-deductible.",
                config => config.Taxes.DeductibleBuildingExpenses,
                (config, value) => config.Taxes.DeductibleBuildingExpenses = value)
            .AddCheckbox(
                () => "Deductible Seed Expenses",
                () => "Whether or not any gold spent on seed purchases should be tax-deductible.",
                config => config.Taxes.DeductibleSeedExpenses,
                (config, value) => config.Taxes.DeductibleSeedExpenses = value)
            .AddCheckbox(
                () => "Deductible Tool Expenses",
                () => "Whether or not any gold spent on tool purchases and upgrades should be tax-deductible.",
                config => config.Taxes.DeductibleToolExpenses,
                (config, value) => config.Taxes.DeductibleToolExpenses = value)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Property Tax")
            .AddNumberField(
                () => "Lateness Fine",
                () => "A flat rate, charged once over the unpaid amount, when income taxes are not paid on-time.",
                config => config.Taxes.PropertyTaxLatenessFine,
                (config, value) => config.Taxes.PropertyTaxLatenessFine = value,
                0f,
                1f,
                0.05f)
            .AddNumberField(
                () => "Used Tile Tax Rate",
                () => "The tax rate charged over a tile actively used for agriculture, livestock or forestry.",
                config => config.Taxes.UsedTileTaxRate,
                (config, value) => config.Taxes.UsedTileTaxRate = value,
                0f,
                0.5f,
                0.01f)
            .AddNumberField(
                () => "Unused Tile Tax Rate",
                () => "The tax rate charged over a tile not actively used for productive means.",
                config => config.Taxes.UnusedTileTaxRate,
                (config, value) => config.Taxes.UnusedTileTaxRate = value,
                0f,
                0.5f,
                0.01f)
            .AddNumberField(
                () => "Building Tile Tax Rate",
                () => "The tax rate charged over a tile occupied by a constructed building (magical buildings are.",
                config => config.Taxes.BuildingTaxRate,
                (config, value) => config.Taxes.BuildingTaxRate = value,
                0f,
                0.5f,
                0.01f)
            .AddCheckbox(
                () => "Exempt Magical Buildings",
                () => "Whether or not magical buildings (i.e., those summoned at the Wizard's Tower) are exempt from property taxes.",
                config => config.Taxes.ExemptMagicalBuilding,
                (config, value) => config.Taxes.ExemptMagicalBuilding = value);
    }
}
