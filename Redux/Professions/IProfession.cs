namespace DaLion.Redux.Professions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions;

#endregion using directives

/// <summary>Interface for all of the <see cref="Farmer"/>'s professions.</summary>
public interface IProfession
{
    /// <summary>Gets a string that uniquely identifies this profession.</summary>
    string StringId { get; }

    /// <summary>Gets the localized and gendered name for this profession.</summary>
    string DisplayName { get; }

    /// <summary>Gets the index used in-game to track professions acquired by the player.</summary>
    int Id { get; }

    /// <summary>Gets the level at which this profession is offered.</summary>
    /// <remarks>Either 5 or 10.</remarks>
    int Level { get; }

    /// <summary>Gets the <see cref="ISkill"/> which offers this profession.</summary>
    ISkill Skill { get; }

    /// <summary>Gets get the professions which branch off from this profession, if any.</summary>
    IEnumerable<int> BranchingProfessions =>
        this.Level != 5 || !this.Skill.ProfessionPairs.TryGetValue(this.Id, out var pair)
            ? Enumerable.Empty<int>()
            : pair.First.Id.Collect(pair.Second.Id);

    /// <summary>Get the localized description text for this profession.</summary>
    /// <param name="prestiged">Whether to get the prestiged or normal variant.</param>
    /// <returns>A human-readability <see cref="string"/> description of the profession.</returns>
    string GetDescription(bool prestiged = false);
}
