namespace DaLion.Redux.Framework;

/// <summary>Holds the IDs of data fields used by all the modules.</summary>
internal sealed class DataFields
{
    #region arsenal

    internal const string Carved = "Carved";

    #endregion arsenal

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

    #region professions

    // player
    internal const string EcologistItemsForaged = "EcologistItemsForaged";
    internal const string GemologistMineralsCollected = "GemologistMineralsCollected";
    internal const string ProspectorHuntStreak = "ProspectorHuntStreak";
    internal const string ScavengerHuntStreak = "ScavengerHuntStreak";
    internal const string ConservationistTrashCollectedThisSeason = "ConservationistTrashCollectedThisSeason";
    internal const string ConservationistActiveTaxBonusPct = "ConservationistActiveTaxBonusPct";
    internal const string UltimateIndex = "UltimateIndex";
    internal const string ForgottenRecipesDict = "ForgottenRecipesDict";

    // monsters
    internal const string Jumping = "Jumping";
    internal const string Target = "Target";
    internal const string Stolen = "Stolen";

    #endregion professions

    #region rings

    internal const string ResonantCooldownReduction = "ResonantCooldownReduction";
    internal const string ResonantDefense = "ResonantDefense";
    internal const string ResonantSpeed = "ResonantSpeed";

    internal const string ResonantWeaponDamage = "ResonantWeaponDamage";
    internal const string ResonantWeaponCritChance = "ResonantWeaponCritChance";
    internal const string ResonantWeaponCritPower = "ResonantWeaponCritPower";
    internal const string ResonantWeaponKnockback = "ResonantWeaponKnockback";
    internal const string ResonantWeaponCooldownReduction = "ResonantWeaponCooldownReduction";
    internal const string ResonantWeaponSpeed = "ResonantWeaponSpeed";
    internal const string ResonantWeaponDefense = "ResonantWeaponDefense";

    internal const string ResonantSlingshotDamage = "ResonantSlingshotDamage";
    internal const string ResonantSlingshotCritChance = "ResonantSlingshotCritChance";
    internal const string ResonantSlingshotCritPower = "ResonantSlingshotCritPower";
    internal const string ResonantSlingshotKnockback = "ResonantSlingshotKnockback";
    internal const string ResonantSlingshotCooldownReduction = "ResonantSlingshotCooldownReduction";
    internal const string ResonantSlingshotSpeed = "ResonantSlingshotSpeed";
    internal const string ResonantSlingshotDefense = "ResonantSlingshotDefense";

    #endregion rings

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
