namespace DaLion.Taxes.Framework.Integrations;

public interface IProfessionsApi
{
    public interface IProfessionsConfig
    {
        uint ConservationistTrashNeededPerTaxDeduction { get; }

        float ConservationistTaxDeductionCeiling { get; }
    }

    float GetConservationistTaxDeduction(Farmer? farmer = null);

    IProfessionsConfig GetConfig();
}
