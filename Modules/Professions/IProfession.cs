namespace DaLion.Overhaul.Modules.Professions;

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

    /// <summary>Gets the index used in-game to track professions acquired by the player.</summary>
    int Id { get; }

    /// <summary>Gets the localized and gendered title for this profession.</summary>
    string Title { get; }

    /// <summary>Gets the localized description text for this profession.</summary>
    string Description { get; }

    /// <summary>Gets the level at which this profession is offered.</summary>
    /// <remarks>Either 5 or 10.</remarks>
    int Level { get; }

    /// <summary>Gets the <see cref="ISkill"/> which offers this profession.</summary>
    ISkill ParentSkill { get; }

    /// <summary>Gets get the professions which branch off from this profession, if any.</summary>
    IEnumerable<IProfession> BranchingProfessions =>
        this.Level != 5 || !this.ParentSkill.ProfessionPairs.TryGetValue(this.Id, out var pair)
            ? Enumerable.Empty<IProfession>()
            : pair.First.Collect(pair.Second);
}
