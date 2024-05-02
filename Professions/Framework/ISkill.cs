﻿namespace DaLion.Professions.Framework;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using StardewValley;

#endregion using directives

/// <summary>Interface for all the <see cref="Farmer"/>'s skills.</summary>
public interface ISkill
{
    /// <summary>The vanilla experience cap.</summary>
    public const int LEVEL_10_EXP = 15000;

    /// <summary>Gets a string that uniquely identifies this skill.</summary>
    string StringId { get; }

    /// <summary>Gets the index used in-game to identify this skill.</summary>
    int Id { get; }

    /// <summary>Gets the localized in-game name of this skill.</summary>
    string DisplayName { get; }

    /// <summary>Gets the current experience total gained by the local player for this skill.</summary>
    int CurrentExp { get; }

    /// <summary>Gets the current level for this skill.</summary>
    int CurrentLevel { get; }

    /// <summary>Gets the highest allowed level for this skill.</summary>
    int MaxLevel { get; }

    /// <summary>Gets the amount of experience required for the next level-up.</summary>
    int ExperienceToNextLevel => this.CurrentLevel == this.MaxLevel ? 0 : ExperienceCurve[this.CurrentLevel + 1];

    /// <summary>Gets the amount of experience required to reach the max level.</summary>
    int ExperienceToMaxLevel => ExperienceCurve[this.MaxLevel];

    /// <summary>Gets the base experience multiplier set by the player for this skill.</summary>
    float BaseExperienceMultiplier { get; }

    /// <summary>Gets the experience multiplier due to this skill's prestige level.</summary>
    float BonusExperienceMultiplier =>
        (float)Math.Pow(Config.Skills.SkillExpMultiplierPerReset, this.AcquiredProfessions.Length);

    /// <summary>Gets the new levels gained during the current game day, which have not yet been accomplished by an overnight menu.</summary>
    IEnumerable<int> NewLevels { get; }

    /// <summary>Gets the <see cref="IProfession"/>s associated with this skill.</summary>
    IList<IProfession> Professions { get; }

    /// <summary>Gets integer ids used in-game to track professions acquired by the player.</summary>
    IEnumerable<int> ProfessionIds => this.Professions.Select(p => p.Id);

    /// <summary>Gets subset of <see cref="ProfessionIds"/> containing only the level five profession ids.</summary>
    /// <remarks>Should always contain exactly 2 elements.</remarks>
    IEnumerable<int> TierOneProfessionIds => this.ProfessionIds.Take(2);

    /// <summary>Gets subset of <see cref="ProfessionIds"/> containing only the level ten profession ids.</summary>
    /// <remarks>
    ///     Should always contains exactly 4 elements. The elements are assumed to be ordered correctly with respect to
    ///     <see cref="TierOneProfessionIds"/>, such that elements 0 and 1 in this array correspond to branches of element 0
    ///     in the latter, and elements 2 and 3 correspond to branches of element 1.
    /// </remarks>
    IEnumerable<int> TierTwoProfessionIds => this.ProfessionIds.TakeLast(4);

    /// <summary>Gets a dictionary of the <see cref="ProfessionPair"/>s offered by this skill indexed by the corresponding level 5 profession.</summary>
    IDictionary<IProfession, ProfessionPair> ProfessionPairByRoot { get; }

    /// <summary>Gets the local player's acquired professions from this skill.</summary>
    IProfession[] AcquiredProfessions => Game1.player.GetProfessionsForSkill(this, true);

    /// <summary>Gets the experience required for each level up.</summary>
    internal static int[] ExperienceCurve { get; } =
    [
        0,
        100,
        380,
        770,
        1300,
        2150,
        3300,
        4800,
        6900,
        10000,
        LEVEL_10_EXP,
        LEVEL_10_EXP + (int)Config.Masteries.ExpPerPrestigeLevel,
        LEVEL_10_EXP + ((int)Config.Masteries.ExpPerPrestigeLevel * 2),
        LEVEL_10_EXP + ((int)Config.Masteries.ExpPerPrestigeLevel * 3),
        LEVEL_10_EXP + ((int)Config.Masteries.ExpPerPrestigeLevel * 4),
        LEVEL_10_EXP + ((int)Config.Masteries.ExpPerPrestigeLevel * 5),
        LEVEL_10_EXP + ((int)Config.Masteries.ExpPerPrestigeLevel * 6),
        LEVEL_10_EXP + ((int)Config.Masteries.ExpPerPrestigeLevel * 7),
        LEVEL_10_EXP + ((int)Config.Masteries.ExpPerPrestigeLevel * 8),
        LEVEL_10_EXP + ((int)Config.Masteries.ExpPerPrestigeLevel * 9),
        LEVEL_10_EXP + ((int)Config.Masteries.ExpPerPrestigeLevel * 10),
    ];

    /// <summary>Adds experience points for this skill.</summary>
    /// <param name="amount">The amount of experience to add.</param>
    void AddExperience(int amount);

    /// <summary>Sets the level of this skill.</summary>
    /// <param name="level">The new level.</param>
    /// <remarks>Will not affect professions or recipes.</remarks>
    void SetLevel(int level);

    /// <summary>Determines whether this skill can be reset for Masteries.</summary>
    /// <returns><see langword="true"/> if the local player meets all reset conditions, otherwise <see langword="false"/>.</returns>
    bool CanReset()
    {
        var farmer = Game1.player;

        var isSkillLevelTen = this.CurrentLevel >= 10;
        if (!isSkillLevelTen)
        {
            Log.D($"[Masteries]: {this.StringId} skill cannot be reset because it's level is lower than 10.");
            return false;
        }

        var justLeveledUp = this.NewLevels.Contains(10);
        if (justLeveledUp)
        {
            Log.D($"[Masteries]: {this.StringId} skill cannot be reset because {farmer.Name} has not yet seen the level-up menu.");
            return false;
        }

        var hasProfessionsLeftToAcquire = farmer.GetProfessionsForSkill(this, true).Length.IsIn(1..3);
        if (!hasProfessionsLeftToAcquire)
        {
            Log.D(
                $"[Masteries]: {this.StringId} skill cannot be reset because {farmer.Name} either already has all professions in the skill, or has none at all.");
            return false;
        }

        var alreadyResetThisSkill = State.SkillsToReset.Contains(this);
        if (alreadyResetThisSkill)
        {
            Log.D($"[Masteries]: {this.StringId} skill has already been marked for reset tonight.");
            return false;
        }

        return true;
    }

    /// <summary>Gets the cost of resetting this skill.</summary>
    /// <returns>A sum of gold to be paid.</returns>
    int GetResetCost()
    {
        var multiplier = Config.Skills.SkillResetCostMultiplier;
        if (multiplier <= 0f)
        {
            return 0;
        }

        var baseCost = this.AcquiredProfessions.Length switch
        {
            1 => 10000,
            2 => 50000,
            3 => 100000,
            _ => 0,
        };

        return (int)(baseCost * multiplier);
    }

    /// <summary>Resets the skill for Masteries.</summary>
    void Reset();

    /// <summary>Removes all recipes associated with this skill from the local player.</summary>
    void ForgetRecipes();

    /// <summary>Determines whether this skill can gain Masteries Levels.</summary>
    /// <returns><see langword="true"/> if the local player meets all Masteries conditions, otherwise <see langword="false"/>.</returns>
    bool CanGainPrestigeLevels();

    /// <summary>Determines whether this skill's level matches the expected level for the current experience, and if not fixes those levels.</summary>
    void Revalidate();

    /// <summary>Determines whether any skill at all can be reset for Masteries.</summary>
    /// <returns><see langword="true"/> if at least one vanilla or loaded custom skill can be reset, otherwise <see langword="false"/>.</returns>
    internal static bool CanResetAny()
    {
        return Skill.List.Any(s => ((ISkill)s).CanReset()) ||
               CustomSkill.Loaded.Values.Any(s => ((ISkill)s).CanReset());
    }

    /// <summary>Revalidates all vanilla and custom skills.</summary>
    internal static void RevalidateAll()
    {
        Skill.List.ForEach(s => s.Revalidate());
        CustomSkill.Loaded.Values.ForEach(s => s.Revalidate());
    }
}