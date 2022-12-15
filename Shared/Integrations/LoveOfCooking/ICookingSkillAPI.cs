#pragma warning disable CS1591
#pragma warning disable SA1602 // Enumeration items should be documented
namespace DaLion.Shared.Integrations.LoveOfCooking;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The API provided by Love Of Cooking.</summary>
public interface ICookingSkillApi
{
    public enum Profession
    {
        ImprovedOil,
        Restoration,
        GiftBoost,
        SalePrice,
        ExtraPortion,
        BuffDuration,
    }

    bool IsEnabled();

    int GetLevel();

    int GetMaximumLevel();

    IReadOnlyDictionary<Profession, bool> GetCurrentProfessions(long playerID = -1L);

    bool HasProfession(Profession profession, long playerID = -1L);

    bool AddExperienceDirectly(int experience);

    void AddCookingBuffToItem(string name, int value);

    int GetTotalCurrentExperience();

    int GetExperienceRequiredForLevel(int level);

    int GetTotalExperienceRequiredForLevel(int level);

    int GetExperienceRemainingUntilLevel(int level);

    IReadOnlyDictionary<int, IList<string>> GetAllLevelUpRecipes();

    IReadOnlyList<string> GetCookingRecipesForLevel(int level);

    int CalculateExperienceGainedFromCookingItem(Item item, int numIngredients, int numCooked, bool applyExperience);

    bool RollForExtraPortion();
}
