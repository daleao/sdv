namespace DaLion.Stardew.Professions.Framework;

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Interface for all of the <see cref="StardewValley.Farmer"/>'s skills.</summary>
public interface ISkill
{
    /// <summary>The skill's unique string id.</summary>
    string StringId { get; }

    /// <summary>The localized in-game name of this skill.</summary>
    string DisplayName { get; }

    /// <summary>The current experience total gained by the local player for this skill.</summary>
    int CurrentExp { get; }

    /// <summary>The current level for this skill.</summary>
    int CurrentLevel { get; }

    /// <summary>The new levels gained during the current game day, which have not yet been accomplished by an overnight menu.</summary>
    IEnumerable<int> NewLevels { get; }

    /// <summary>The <see cref="IProfession"/>s associated with this skill.</summary>
    IList<IProfession> Professions { get; }

    /// <summary>The <see cref="ProfessionPair"/>s offered by this skill.</summary>
    IDictionary<int, ProfessionPair> ProfessionPairs { get; }

    /// <summary>Integer ids used in-game to track professions acquired by the player.</summary>
    IEnumerable<int> ProfessionIds => Professions.Select(p => p.Id);

    /// <summary>Subset of <see cref="ProfessionIds"/> containing only the level five profession ids.</summary>
    /// <remarks>Should always contain exactly 2 elements.</remarks>
    virtual IEnumerable<int> TierOneProfessionIds => ProfessionIds.Take(2);

    /// <summary>Subset of <see cref="ProfessionIds"/> containing only the level ten profession ids.</summary>
    /// <remarks>Should always contains exactly 4 elements. The elements are assumed to be ordered correctly with respect to <see cref="TierOneProfessionIds"/>, such that elements 0 and 1 in this array correspond to branches of element 0 in the latter, and elements 2 and 3 correspond to branches of element 1.</remarks>
    virtual IEnumerable<int> TierTwoProfessionIds => ProfessionIds.TakeLast(4);
}