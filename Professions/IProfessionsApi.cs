namespace DaLion.Professions;

/// <summary>The public interface for the Professions mod API.</summary>
public interface IProfessionsApi
{
    #region professions

    /// <summary>Gets the value of an Ecologist's forage quality.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    int GetEcologistForageQuality(Farmer? farmer = null);

    /// <summary>Gets the value of a Gemologist's mineral quality.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    int GetGemologistMineralQuality(Farmer? farmer = null);

    /// <summary>Gets the price bonus applied to animal produce sold by Producer.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A bonus applied to Producer animal product prices.</returns>
    float GetProducerSaleBonus(Farmer? farmer = null);

    /// <summary>Gets the price bonus applied to fish sold by Angler.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A bonus applied to Angler fish prices.</returns>
    float GetAnglerSaleBonus(Farmer? farmer = null);

    /// <summary>
    ///     Gets the value of the Conservationist's effective tax deduction based on the preceding season's trash
    ///     collection.
    /// </summary>
    /// <param name="farmer">The player.</param>
    /// <returns>The percentage of tax deductions currently in effect due to the preceding season's collected trash.</returns>
    float GetConservationistTaxDeduction(Farmer? farmer = null);

    /// <summary>Determines the extra power of Desperado shots.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A percentage between 0 and 1.</returns>
    float GetDesperadoOvercharge(Farmer? farmer = null);

    #endregion professions

    /// <summary>Gets the mod's current config schema.</summary>
    /// <returns>The current config instance.</returns>
    ProfessionsConfig GetConfig();
}
