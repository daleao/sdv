namespace DaLion.Stardew.Professions.Framework;

#region using directives

using Common.Extensions;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Interface for all of the <see cref="StardewValley.Farmer"/>'s professions.</summary>
public interface IProfession
{
    /// <summary>The string that uniquely identifies this profession.</summary>
    string StringId { get; }

    /// <summary>The index used in-game to track professions acquired by the player.</summary>
    int Id { get; }

    /// <summary>The level at which this profession is offered.</summary>
    /// <remarks>Either <c>5</c> or <c>10</c>.</remarks>
    int Level { get; }

    /// <summary>The <see cref="ISkill"/> which offers this profession.</summary>
    ISkill Skill { get; }

    /// <summary>Get the localized and gendered name for this profession.</summary>
    /// <param name="male">Whether to get the male or female variant..</param>
    string GetDisplayName(bool male = true);

    /// <summary>Get the description text for this profession.</summary>
    /// <param name="prestiged">Whether to get the prestiged or normal variant.</param>
    string GetDescription(bool prestiged = false);

    /// <summary>Get the professions which branch off from this profession, if any.</summary>
    IEnumerable<int> BranchingProfessions =>
        Level != 5 || !Skill.ProfessionPairs.TryGetValue(Id, out var pair)
        ? Enumerable.Empty<int>()
        : pair.First.Id.Collect(pair.Second.Id);
}