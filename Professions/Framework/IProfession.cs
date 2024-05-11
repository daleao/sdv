namespace DaLion.Professions.Framework;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Extensions;

#endregion using directives

/// <summary>Interface for all the <see cref="Farmer"/>'s professions.</summary>
public interface IProfession : IComparable<IProfession>, IEquatable<IProfession>
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
    IEnumerable<IProfession> GetBranchingProfessions =>
        this.Level == 5 && this.ParentSkill.ProfessionPairByRoot.TryGetValue(this, out var pair)
            ? pair.First.Collect(pair.Second)
            : [];

    /// <summary>Gets get the profession from which this profession branches off of, if any.</summary>
    IProfession? GetRootProfession =>
        this.ParentSkill.ProfessionPairByRoot.Values.Single(pair => this == pair.First || this == pair.Second).Root;
}
