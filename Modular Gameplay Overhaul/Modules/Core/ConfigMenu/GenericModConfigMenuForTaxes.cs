namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

using System.Linq;

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuCore
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
                20f,
                0.5f)
            .AddCheckbox(
                () => "Deductible Building Expenses",
                () => "Whether or not any gold spent constructing farm buildings is tax-deductible.",
                config => config.Taxes.DeductibleBuildingExpenses,
                (config, value) => config.Taxes.DeductibleBuildingExpenses = value)
            .AddCheckbox(
                () => "Deductible Tool Expenses",
                () => "Whether or not any gold spent upgrading tools is tax-deductible.",
                config => config.Taxes.DeductibleBuildingExpenses,
                (config, value) => config.Taxes.DeductibleBuildingExpenses = value)

            .AddSectionTitle(() => "Property Tax");
    }
}
