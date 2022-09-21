namespace DaLion.Stardew.Professions.Framework;

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Interface for all of the <see cref="StardewValley.Farmer"/>'s skills.</summary>
public interface ISkill
{
    internal const int VanillaExpCap = 15000;

    /// <summary>Gets the skill's unique string id.</summary>
    string StringId { get; }

    /// <summary>Gets the localized in-game name of this skill.</summary>
    string DisplayName { get; }

    /// <summary>Gets the current experience total gained by the local player for this skill.</summary>
    int CurrentExp { get; }

    /// <summary>Gets the current level for this skill.</summary>
    int CurrentLevel { get; }

    /// <summary>Gets the amount of experience required for the next level-up.</summary>
    int ExperienceToNextLevel => this.CurrentLevel == MaxLevel ? 0 : ExperienceByLevel[this.CurrentLevel + 1];

    /// <summary>Gets the base experience multiplier set by the player for this skill.</summary>
    float BaseExperienceMultiplier { get; }

    /// <summary>Gets the new levels gained during the current game day, which have not yet been accomplished by an overnight menu.</summary>
    IEnumerable<int> NewLevels { get; }

    /// <summary>Gets the <see cref="IProfession"/>s associated with this skill.</summary>
    IList<IProfession> Professions { get; }

    /// <summary>Gets the <see cref="ProfessionPair"/>s offered by this skill.</summary>
    IDictionary<int, ProfessionPair> ProfessionPairs { get; }

    /// <summary>Gets integer ids used in-game to track professions acquired by the player.</summary>
    IEnumerable<int> ProfessionIds => this.Professions.Select(p => p.Id);

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

    internal static int MaxLevel => ModEntry.Config.EnablePrestige ? 20 : 10;

    internal static Dictionary<int, int> ExperienceByLevel { get; } = new()
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
        { 10, VanillaExpCap },
        { 11, VanillaExpCap + (int)ModEntry.Config.RequiredExpPerExtendedLevel },
        { 12, VanillaExpCap + ((int)ModEntry.Config.RequiredExpPerExtendedLevel * 2) },
        { 13, VanillaExpCap + ((int)ModEntry.Config.RequiredExpPerExtendedLevel * 3) },
        { 14, VanillaExpCap + ((int)ModEntry.Config.RequiredExpPerExtendedLevel * 4) },
        { 15, VanillaExpCap + ((int)ModEntry.Config.RequiredExpPerExtendedLevel * 5) },
        { 16, VanillaExpCap + ((int)ModEntry.Config.RequiredExpPerExtendedLevel * 6) },
        { 17, VanillaExpCap + ((int)ModEntry.Config.RequiredExpPerExtendedLevel * 7) },
        { 18, VanillaExpCap + ((int)ModEntry.Config.RequiredExpPerExtendedLevel * 8) },
        { 19, VanillaExpCap + ((int)ModEntry.Config.RequiredExpPerExtendedLevel * 9) },
        { 20, VanillaExpCap + ((int)ModEntry.Config.RequiredExpPerExtendedLevel * 10) },
    };
}
