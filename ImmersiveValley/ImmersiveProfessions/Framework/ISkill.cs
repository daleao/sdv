namespace DaLion.Stardew.Professions.Framework;

#region using directives

using LinqFasterer;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Interface for all of the <see cref="StardewValley.Farmer"/>'s skills.</summary>
public interface ISkill
{
    internal const int VANILLA_EXP_CAP_I = 15000;

    /// <summary>The skill's unique string id.</summary>
    string StringId { get; }

    /// <summary>The localized in-game name of this skill.</summary>
    string DisplayName { get; }

    /// <summary>The current experience total gained by the local player for this skill.</summary>
    int CurrentExp { get; }

    /// <summary>The current level for this skill.</summary>
    int CurrentLevel { get; }

    /// <summary>The amount of experience required for the next level-up.</summary>
    int ExperienceToNextLevel => CurrentLevel == MaxLevel ? 0 : ExperienceByLevel[CurrentLevel + 1];

    /// <summary>The base experience multiplier set by the player for this skill.</summary>
    float BaseExperienceMultiplier { get; }

    /// <summary>The new levels gained during the current game day, which have not yet been accomplished by an overnight menu.</summary>
    IEnumerable<int> NewLevels { get; }

    /// <summary>The <see cref="IProfession"/>s associated with this skill.</summary>
    IList<IProfession> Professions { get; }

    /// <summary>The <see cref="ProfessionPair"/>s offered by this skill.</summary>
    IDictionary<int, ProfessionPair> ProfessionPairs { get; }

    /// <summary>Integer ids used in-game to track professions acquired by the player.</summary>
    IEnumerable<int> ProfessionIds => Professions.SelectF(p => p.Id);

    /// <summary>Subset of <see cref="ProfessionIds"/> containing only the level five profession ids.</summary>
    /// <remarks>Should always contain exactly 2 elements.</remarks>
    virtual IEnumerable<int> TierOneProfessionIds => ProfessionIds.Take(2);

    /// <summary>Subset of <see cref="ProfessionIds"/> containing only the level ten profession ids.</summary>
    /// <remarks>Should always contains exactly 4 elements. The elements are assumed to be ordered correctly with respect to <see cref="TierOneProfessionIds"/>, such that elements 0 and 1 in this array correspond to branches of element 0 in the latter, and elements 2 and 3 correspond to branches of element 1.</remarks>
    virtual IEnumerable<int> TierTwoProfessionIds => ProfessionIds.TakeLast(4);

    #region static properties

    internal static int MaxLevel => ModEntry.Config.EnablePrestige ? 20 : 10;

    internal static Dictionary<int, int> ExperienceByLevel = new()
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
        { 10, VANILLA_EXP_CAP_I },
        { 11, VANILLA_EXP_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel },
        { 12, VANILLA_EXP_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 2 },
        { 13, VANILLA_EXP_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 3 },
        { 14, VANILLA_EXP_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 4 },
        { 15, VANILLA_EXP_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 5 },
        { 16, VANILLA_EXP_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 6 },
        { 17, VANILLA_EXP_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 7 },
        { 18, VANILLA_EXP_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 8 },
        { 19, VANILLA_EXP_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 9 },
        { 20, VANILLA_EXP_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 10 }
    };

    #endregion static properties
}