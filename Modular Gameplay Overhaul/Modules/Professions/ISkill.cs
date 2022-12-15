namespace DaLion.Overhaul.Modules.Professions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Collections;
using StardewValley;

#endregion using directives

/// <summary>Interface for all of the <see cref="Farmer"/>'s skills.</summary>
public interface ISkill
{
    /// <summary>Gets the skill's unique string id.</summary>
    string StringId { get; }

    /// <summary>Gets the localized in-game name of this skill.</summary>
    string DisplayName { get; }

    /// <summary>Gets the current experience total gained by the local player for this skill.</summary>
    int CurrentExp { get; }

    /// <summary>Gets the current level for this skill.</summary>
    int CurrentLevel { get; }

    /// <summary>Gets the highest allowed level for this skill.</summary>
    int MaxLevel { get; }

    /// <summary>Gets the amount of experience required for the next level-up.</summary>
    int ExperienceToNextLevel => this.CurrentLevel == this.MaxLevel ? 0 : ExperienceByLevel[this.CurrentLevel + 1];

    /// <summary>Gets the base experience multiplier set by the player for this skill.</summary>
    float BaseExperienceMultiplier { get; }

    /// <summary>Gets this skill's prestige level.</summary>
    int PrestigeLevel => Game1.player.GetProfessionsForSkill(this, true).Length;

    /// <summary>Gets the experience multiplier due to this skill's prestige level.</summary>
    float PrestigeExperienceMultiplier =>
        (float)Math.Pow(1f + ProfessionsModule.Config.PrestigeExpMultiplier, this.PrestigeLevel);

    /// <summary>Gets the new levels gained during the current game day, which have not yet been accomplished by an overnight menu.</summary>
    IEnumerable<int> NewLevels { get; }

    /// <summary>Gets the <see cref="IProfession"/>s associated with this skill.</summary>
    IList<IProfession> Professions { get; }

    /// <summary>Gets the <see cref="ProfessionPair"/>s offered by this skill.</summary>
    IDictionary<int, ProfessionPair> ProfessionPairs { get; }

    /// <summary>Gets integer ids used in-game to track professions acquired by the player.</summary>
    virtual IEnumerable<int> ProfessionIds => this.Professions.Select(p => p.Id);

    /// <summary>Gets subset of <see cref="ProfessionIds"/> containing only the level five profession ids.</summary>
    /// <remarks>Should always contain exactly 2 elements.</remarks>
    virtual IEnumerable<int> TierOneProfessionIds => this.ProfessionIds.Take(2);

    /// <summary>Gets subset of <see cref="ProfessionIds"/> containing only the level ten profession ids.</summary>
    /// <remarks>
    ///     Should always contains exactly 4 elements. The elements are assumed to be ordered correctly with respect to
    ///     <see cref="TierOneProfessionIds"/>, such that elements 0 and 1 in this array correspond to branches of element 0
    ///     in the latter, and elements 2 and 3 correspond to branches of element 1.
    /// </remarks>
    virtual IEnumerable<int> TierTwoProfessionIds => this.ProfessionIds.TakeLast(4);

    /// <summary>Gets the experience required for each level.</summary>
    internal static IReadOnlyDictionary<int, int> ExperienceByLevel { get; } = new Dictionary<int, int>
    {
        { 0, 0 },
        { 1, 100 },
        { 2, 380 },
        { 3, 770 },
        { 4, 1300 },
        { 5, 2150 },
        { 6, 3300 },
        { 7, 4800 },
        { 8, 6900 },
        { 9, 10000 },
        { 10, Constants.ExpAtLevel10 },
        { 11, Constants.ExpAtLevel10 + (int)ProfessionsModule.Config.RequiredExpPerExtendedLevel },
        { 12, Constants.ExpAtLevel10 + ((int)ProfessionsModule.Config.RequiredExpPerExtendedLevel * 2) },
        { 13, Constants.ExpAtLevel10 + ((int)ProfessionsModule.Config.RequiredExpPerExtendedLevel * 3) },
        { 14, Constants.ExpAtLevel10 + ((int)ProfessionsModule.Config.RequiredExpPerExtendedLevel * 4) },
        { 15, Constants.ExpAtLevel10 + ((int)ProfessionsModule.Config.RequiredExpPerExtendedLevel * 5) },
        { 16, Constants.ExpAtLevel10 + ((int)ProfessionsModule.Config.RequiredExpPerExtendedLevel * 6) },
        { 17, Constants.ExpAtLevel10 + ((int)ProfessionsModule.Config.RequiredExpPerExtendedLevel * 7) },
        { 18, Constants.ExpAtLevel10 + ((int)ProfessionsModule.Config.RequiredExpPerExtendedLevel * 8) },
        { 19, Constants.ExpAtLevel10 + ((int)ProfessionsModule.Config.RequiredExpPerExtendedLevel * 9) },
        { 20, Constants.ExpAtLevel10 + ((int)ProfessionsModule.Config.RequiredExpPerExtendedLevel * 10) },
    };

    /// <summary>Adds experience points for this skill.</summary>
    /// <param name="amount">The amount of experience to add.</param>
    void AddExperience(int amount);

    /// <summary>Sets the level of this skill.</summary>
    /// <param name="level">The new level.</param>
    /// <remarks>Will not affect professions or recipes.</remarks>
    void SetLevel(int level);

    /// <summary>Determines whether this skill can be reset for prestige.</summary>
    /// <returns><see langword="true"/> if the local player meets all reset conditions, otherwise <see langword="false"/>.</returns>
    virtual bool CanReset()
    {
        var farmer = Game1.player;

        var isSkillLevelTen = this.CurrentLevel >= 10;
        if (!isSkillLevelTen)
        {
            Log.D($"{this.StringId} skill cannot be reset because it's level is lower than 10.");
            return false;
        }

        var justLeveledUp = this.NewLevels.Contains(10);
        if (justLeveledUp)
        {
            Log.D($"{this.StringId} skill cannot be reset because {farmer.Name} has not yet seen the level-up menu.");
            return false;
        }

        var hasProfessionsLeftToAcquire =
            farmer.GetProfessionsForSkill(this, true).Length is > 0 and < 4;
        if (!hasProfessionsLeftToAcquire)
        {
            Log.D(
                $"{this.StringId} skill cannot be reset because {farmer.Name} either already has all professions in the skill, or has none at all.");
            return false;
        }

        var alreadyResetThisSkill = ProfessionsModule.State.SkillsToReset.Contains(this);
        if (alreadyResetThisSkill)
        {
            Log.D($"{this.StringId} skill has already been marked for reset tonight.");
            return false;
        }

        return true;
    }

    /// <summary>Gets the cost of resetting this skill.</summary>
    /// <returns>A sum of gold to be paid.</returns>
    virtual int GetResetCost()
    {
        var multiplier = ProfessionsModule.Config.SkillResetCostMultiplier;
        if (multiplier <= 0f)
        {
            return 0;
        }

        var baseCost = this.PrestigeLevel switch
        {
            1 => 10000,
            2 => 50000,
            3 => 100000,
            _ => 0,
        };

        return (int)(baseCost * multiplier);
    }

    /// <summary>Resets the skill for prestige.</summary>
    void Reset();

    /// <summary>Removes all recipes associated with this skill from the local player.</summary>
    /// <param name="saveForRecovery">Whether to store crafted quantities for later recovery.</param>
    void ForgetRecipes(bool saveForRecovery = true);

    /// <summary>Determines whether this skill's level matches the expected level for the current experience, and if not fixes those levels.</summary>
    void Revalidate();

    /// <summary>Determines whether any skill at all can be reset for prestige.</summary>
    /// <returns><see langword="true"/> if at least one vanilla or loaded custom skill can be reset, otherwise <see langword="false"/>.</returns>
    internal static bool CanResetAny()
    {
        return Skill.List.Any(s => ((ISkill)s).CanReset()) || SCSkill.Loaded.Values.Any(s => s.CanReset());
    }

    /// <summary>Revalidates all vanilla and custom skills.</summary>
    internal static void RevalidateAll()
    {
        Skill.List.ForEach(s => s.Revalidate());
        SCSkill.Loaded.Values.ForEach(s => s.Revalidate());
    }
}
