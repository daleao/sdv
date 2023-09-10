namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using System.Linq;

#endregion

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the config menu for TXS.</summary>
    private void AddTaxOptions()
    {
        this
            .AddPage(OverhaulModule.Taxes.Namespace, I18n.Gmcm_Txs_Heading)

            .AddSectionTitle(I18n.Gmcm_Profs_General_Heading)
            .AddNumberField(
                I18n.Gmcm_Txs_Annualinterest_Title,
                I18n.Gmcm_Txs_Annualinterest_Desc,
                config => config.Taxes.AnnualInterest,
                (config, value) => config.Taxes.AnnualInterest = value,
                0f,
                1f,
                0.02f)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Txs_Income_Heading)
            .AddTextbox(
                I18n.Gmcm_Txs_Income_Brackets_Title,
                I18n.Gmcm_Txs_Income_Brackets_Desc,
                config => string.Join(", ", config.Taxes.IncomeBrackets),
                (config, value) => config.Taxes.IncomeBrackets =
                    value.Split(new[] { ", " }, StringSplitOptions.None).Select(int.Parse).ToArray())
            .AddTextbox(
                I18n.Gmcm_Txs_Income_TaxPerBracket_Title,
                I18n.Gmcm_Txs_Income_TaxPerBracket_Desc,
                config => string.Join(", ", config.Taxes.TaxPerBracket),
                (config, value) => config.Taxes.TaxPerBracket =
                    value.Split(new[] { ", " }, StringSplitOptions.None).Select(float.Parse).ToArray())
            .AddNumberField(
                I18n.Gmcm_Txs_Income_LatenessFine_Title,
                I18n.Gmcm_Txs_Income_LatenessFine_Desc,
                config => config.Taxes.IncomeTaxLatenessFine,
                (config, value) => config.Taxes.IncomeTaxLatenessFine = value,
                0f,
                1f,
                0.05f)
            .AddNumberField(
                I18n.Gmcm_Txs_Income_DeductibleAnimalExpenses_Title,
                I18n.Gmcm_Txs_Income_DeductibleAnimalExpenses_Desc,
                config => config.Taxes.DeductibleAnimalExpenses,
                (config, value) => config.Taxes.DeductibleAnimalExpenses = value,
                0f,
                1f,
                0.05f)
            .AddNumberField(
                I18n.Gmcm_Txs_Income_DeductibleBuildingExpenses_Title,
                I18n.Gmcm_Txs_Income_DeductibleBuildingExpenses_Desc,
                config => config.Taxes.DeductibleBuildingExpenses,
                (config, value) => config.Taxes.DeductibleBuildingExpenses = value,
                0f,
                1f,
                0.05f)
            .AddNumberField(
                I18n.Gmcm_Txs_Income_DeductibleSeedExpenses_Title,
                I18n.Gmcm_Txs_Income_DeductibleSeedExpenses_Desc,
                config => config.Taxes.DeductibleSeedExpenses,
                (config, value) => config.Taxes.DeductibleSeedExpenses = value,
                0f,
                1f,
                0.05f)
            .AddNumberField(
                I18n.Gmcm_Txs_Income_DeductibleToolExpenses_Title,
                I18n.Gmcm_Txs_Income_DeductibleToolExpenses_Desc,
                config => config.Taxes.DeductibleToolExpenses,
                (config, value) => config.Taxes.DeductibleToolExpenses = value,
                0f,
                1f,
                0.05f)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Txs_Property_Heading)
            .AddNumberField(
                I18n.Gmcm_Txs_Property_LatenessFine_Title,
                I18n.Gmcm_Txs_Property_LatenessFine_Desc,
                config => config.Taxes.PropertyTaxLatenessFine,
                (config, value) => config.Taxes.PropertyTaxLatenessFine = value,
                0f,
                1f,
                0.05f)
            .AddNumberField(
                I18n.Gmcm_Txs_Property_UsedTileTaxRate_Title,
                I18n.Gmcm_Txs_Property_UsedTileTaxRate_Desc,
                config => config.Taxes.UsedTileTaxRate,
                (config, value) => config.Taxes.UsedTileTaxRate = value,
                0f,
                0.5f,
                0.01f)
            .AddNumberField(
                I18n.Gmcm_Txs_Property_UnusedTileTaxRate_Title,
                I18n.Gmcm_Txs_Property_UnusedTileTaxRate_Desc,
                config => config.Taxes.UnusedTileTaxRate,
                (config, value) => config.Taxes.UnusedTileTaxRate = value,
                0f,
                0.5f,
                0.01f)
            .AddNumberField(
                I18n.Gmcm_Txs_Property_BuildingTileTaxRate_Title,
                I18n.Gmcm_Txs_Property_BuildingTileTaxRate_Desc,
                config => config.Taxes.BuildingTaxRate,
                (config, value) => config.Taxes.BuildingTaxRate = value,
                0f,
                0.5f,
                0.01f)
            .AddCheckbox(
                I18n.Gmcm_Txs_Property_ExemptMagicalBuildings_Title,
                I18n.Gmcm_Txs_Property_ExemptMagicalBuildings_Desc,
                config => config.Taxes.ExemptMagicalBuilding,
                (config, value) => config.Taxes.ExemptMagicalBuilding = value);
    }
}
