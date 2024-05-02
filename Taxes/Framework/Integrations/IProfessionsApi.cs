namespace DaLion.Taxes.Framework.Integrations;

internal interface IProfessionsApi
{
    internal interface IModConfig
    {
        int TrashNeededPerTaxDeduction { get; }

        float ConservationistTaxDeductionCeiling { get; }
    }

    float GetConservationistDeductions();

    IModConfig GetConfigs();
}
