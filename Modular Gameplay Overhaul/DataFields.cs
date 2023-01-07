namespace DaLion.Overhaul;

/// <summary>Holds the string IDs of mod data fields used by all the modules.</summary>
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

    #region arsenal

    // weapon
    internal const string CursePoints = "CursePoints";
    internal const string BaseMinDamage = "BaseMinDamage";
    internal const string BaseMaxDamage = "BaseMaxDamage";

    // farmer
    internal const string GalaxyArsenalObtained = "GalaxyArsenalObtained";
    internal const string Cursed = "Cursed";
    internal const string BlueprintsFound = "BlueprintsFound";
    internal const string DaysLeftTranslating = "DaysLeftTranslating";
    internal const string HasReadHonor = "HasReadHonor";
    internal const string HasReadCompassion = "HasReadCompassion";
    internal const string HasReadWisdom = "HasReadWisdom";
    internal const string HasReadGenerosity = "HasReadGenerosity";
    internal const string HasReadValor = "HasReadValor";
    internal const string ProvenHonor = "ProvenHonor"; // max 8 points (-4)
    internal const string ProvenCompassion = "ProvenCompassion"; // max 9 points (-3)
    internal const string ProvenWisdom = "ProvenWisdom"; // max 8 points (-1)
    internal const string ProvenGenerosity = "ProvenGenerosity"; // awarded before the mail flag `pamHouseUpgrade`
    internal const string ProvenValor = "ProvenValor"; // awarded after 5th monster slayer quest completion

    #endregion arsenal

    #region ponds

    internal const string FishQualities = "FishQualities";
    internal const string FamilyQualities = "FamilyQualities";
    internal const string FamilyLivingHere = "FamilyLivingHere";
    internal const string DaysEmpty = "DaysEmpty";
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
