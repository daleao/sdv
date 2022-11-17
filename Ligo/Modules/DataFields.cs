namespace DaLion.Ligo.Modules;

/// <summary>Holds the IDs of data fields used by all the modules.</summary>
internal sealed class DataFields
{
    #region professions

    internal const string EcologistItemsForaged = "EcologistItemsForaged";
    internal const string GemologistMineralsCollected = "GemologistMineralsCollected";
    internal const string ProspectorHuntStreak = "ProspectorHuntStreak";
    internal const string ScavengerHuntStreak = "ScavengerHuntStreak";
    internal const string ConservationistTrashCollectedThisSeason = "ConservationistTrashCollectedThisSeason";
    internal const string ConservationistActiveTaxBonusPct = "ConservationistActiveTaxBonusPct";
    internal const string UltimateIndex = "UltimateIndex";
    internal const string ForgottenRecipesDict = "ForgottenRecipesDict";

    #endregion professions

    #region rings

    internal const string RingCooldownReduction = "RingCooldownReduction";

    internal const string ResonantDamage = "ResonantDamage";
    internal const string ResonantCritChance = "ResonantCritChance";
    internal const string ResonantCritPower = "ResonantCritPower";
    internal const string ResonantKnockback = "ResonantKnockback";
    internal const string ResonantCooldownReduction = "ResonantCooldownReduction";
    internal const string ResonantSpeed = "ResonantSpeed";
    internal const string ResonantDefense = "ResonantDefense";

    #endregion rings

    #region ponds

    internal const string FishQualities = "FishQualities";
    internal const string FamilyQualities = "FamilyQualities";
    internal const string FamilyLivingHere = "FamilyLivingHere";
    internal const string DaysEmpty = "FamilyLivingHere";
    internal const string SeaweedLivingHere = "SeaweedLivingHere";
    internal const string GreenAlgaeLivingHere = "GreenAlgaeLivingHere";
    internal const string WhiteAlgaeLivingHere = "WhiteAlgaeLivingHere";
    internal const string CheckedToday = "CheckedToday";
    internal const string ItemsHeld = "ItemsHeld";
    internal const string MetalsHeld = "MetalsHeld";

    #endregion ponds

    #region taxes

    internal const string SeasonIncome = "SeasonIncome";
    internal const string BusinessExpenses = "BusinessExpenses";
    internal const string PercentDeductions = "PercentDeductions";
    internal const string DebtOutstanding = "DebtOutstanding";

    #endregion taxes

    #region tweaks

    internal const string Age = "Age";

    #endregion tweaks
}
