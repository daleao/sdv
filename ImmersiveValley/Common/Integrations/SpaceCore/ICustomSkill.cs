#nullable enable
namespace DaLion.Common.Integrations;

#region using directives

using System.Collections.Generic;

#endregion using directives

public interface ICustomSkill
{
    /// <summary>The string that uniquely identifies this skill within SpaceCore's custom skill cache.</summary>
    string StringId { get; }
    
    /// <summary>The localized in-game name of this skill</summary>
    string DisplayName { get; }

    /// <summary>The current experience total gained by the local player for this skill.</summary>
    int CurrentExp { get; }

    /// <summary>The current level for this skill.</summary>
    int CurrentLevel { get; }

    /// <summary>The levels gained during the current game day, which have not yet been accomplished by an overnight menu.</summary>
    int[] NewLevels { get; }

    /// <summary>Array of integer ids used in-game to track professions acquired by the player.</summary>
    int[] ProfessionIds { get; }

    /// <summary>Subset of <see cref="ProfessionIds"/> containing only the level five profession ids.</summary>
    /// <remarks>This array should always contain exactly 2 elements.</remarks>
    int[] TierOneProfessionIds { get; }

    /// <summary>Subset of <see cref="ProfessionIds"/> containing only the level ten profession ids.</summary>
    /// <remarks>This array should always contains exactly 4 elements. The elements are assumed to be ordered correctly with respect to <see cref="TierOneProfessionIds"/>, such that elements 0 and 1 in this array correspond to branches of element 0 in the latter, and elements 2 and 3 correspond to branches of element 1.</remarks>
    int[] TierTwoProfessionIds { get; }

    /// <summary>Look-up for the corresponding level ten professions branching from each level five profession.</summary>
    Dictionary<int, (int first, int second)> ProfessionsByBranch { get; }

    /// <summary>Look-up for the corresponding display name of each profession by its integer id.</summary>
    Dictionary<int, string> ProfessionNamesById { get; }
}