namespace DaLion.Professions;

#region using directives

using StardewValley.Tools;

#endregion using directive

/// <summary>The <see cref="ProfessionsMod"/> API implementation.</summary>
public class ProfessionsApi : IProfessionsApi
{
    #region professions

    /// <inheritdoc />
    public int GetEcologistForageQuality(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.HasProfession(Profession.Ecologist) ? farmer.GetEcologistForageQuality() : SObject.lowQuality;
    }

    /// <inheritdoc />
    public int GetGemologistMineralQuality(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.HasProfession(Profession.Gemologist) ? farmer.GetGemologistMineralQuality() : SObject.lowQuality;
    }

    /// <inheritdoc />
    public float GetProducerSaleBonus(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.GetProducerSaleBonus() + 1f;
    }

    /// <inheritdoc />
    public float GetAnglerSaleBonus(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.GetAnglerSaleBonus() + 1f;
    }

    /// <inheritdoc />
    public float GetConservationistTaxDeduction(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return Data.ReadAs<float>(farmer, DataKeys.ConservationistActiveTaxDeduction);
    }

    /// <inheritdoc />
    public float GetDesperadoOvercharge(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        if (farmer.CurrentTool is not Slingshot slingshot || !farmer.usingSlingshot)
        {
            return 0f;
        }

        return slingshot.GetOvercharge();
    }

    #endregion professions

    /// <inheritdoc />
    public ProfessionsConfig GetConfig()
    {
        return Config;
    }
}
