global using SCProfession = SpaceCore.Skills.Skill.Profession;

namespace DaLion.Professions.Framework;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#endregion using directives

/// <summary>Represents a profession tied to a mod-provided <see cref="ISkill"/>.</summary>
public sealed class CustomProfession : IProfession
{
    private static readonly Dictionary<SCProfession, CustomProfession> FromSpaceCore = [];

    private readonly SCProfession _scProfession;
    private readonly Func<string> _titleGetter;
    private readonly Func<string> _descriptionGetter;

    /// <summary>Initializes a new instance of the <see cref="CustomProfession"/> class.</summary>
    /// <param name="id">The integer id used in-game to track professions acquired by the player.</param>
    /// <param name="stringId">The string used by SpaceCore to uniquely identify this profession.</param>
    /// <param name="getTitle">A function for getting the localized in-game title of this profession.</param>
    /// <param name="getDescription">A function for getting the localized in-game description of this profession.</param>
    /// <param name="level">The level at which this profession is offered.</param>
    /// <param name="skill">The <see cref="CustomSkill"/> to which this profession belongs.</param>
    internal CustomProfession(
        int id,
        string stringId,
        Func<string> getTitle,
        Func<string> getDescription,
        int level,
        CustomSkill skill)
    {
        this.Id = id;
        this._scProfession = skill.ToSpaceCore().Professions.First(p => p.Id == stringId);
        this.StringId = stringId;
        this.Level = level;
        this.ParentSkill = skill;
        this._titleGetter = getTitle;
        this._descriptionGetter = getDescription;
    }

    /// <summary>Initializes a new instance of the <see cref="CustomProfession"/> class from an equivalent <see cref="SCProfession"/>.</summary>
    /// <param name="scProfession">The equivalent <see cref="SCProfession"/>.</param>
    /// <param name="level">The level at which this profession is offered.</param>
    /// <param name="skill">The <see cref="CustomSkill"/> to which this profession belongs.</param>
    internal CustomProfession(SCProfession scProfession, int level, CustomSkill skill)
    {
        this._scProfession = scProfession;
        this.Id = scProfession.GetVanillaId();
        this.StringId = scProfession.Id;
        this.Level = level;
        this.ParentSkill = skill;
        this._titleGetter = scProfession.GetName;
        this._descriptionGetter = scProfession.GetDescription;
        Loaded[this.Id] = this;
        FromSpaceCore[scProfession] = this;
    }

    /// <summary>Enumerates all the loaded instances of <see cref="CustomProfession"/>.</summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors", Justification = "Enumerator.")]
    public static IEnumerable<CustomProfession> List => Loaded.Values;

    /// <inheritdoc />
    public int Id { get; }

    /// <inheritdoc />
    public string StringId { get; }

    /// <inheritdoc />
    public string Title => this._titleGetter.Invoke();

    /// <inheritdoc />
    public string Description => this._descriptionGetter.Invoke();

    /// <inheritdoc />
    public int Level { get; }

    /// <inheritdoc />
    public ISkill ParentSkill { get; }

    /// <summary>Gets professions for loaded <see cref="CustomSkill"/>s.</summary>
    internal static Dictionary<int, CustomProfession> Loaded { get; } = [];

    /// <summary>Enumerates all the ids of loaded <see cref="CustomProfession"/> instances.</summary>
    /// <param name="prestige">Whether to enumerate prestige professions instead.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all loaded SpaceCore profession indices.</returns>
    public static IEnumerable<int> GetAllIds(bool prestige = false)
    {
        return List.Select(p => prestige ? p.Id + 100 : p.Id);
    }

    /// <summary>Gets the <see cref="CustomProfession"/> equivalent to the specified <see cref="SCSkill.Profession"/>.</summary>
    /// <param name="scProfession">The <see cref="SCProfession"/>.</param>
    /// <returns>The equivalent <see cref="CustomProfession"/>.</returns>
    public static CustomProfession? GetFromSpaceCore(SCProfession scProfession)
    {
        return FromSpaceCore.GetValueOrDefault(scProfession);
    }

    /// <inheritdoc />
    public int CompareTo(IProfession? other)
    {
        if (other is null)
        {
            return -1;
        }

        if (this.ParentSkill != other.ParentSkill)
        {
            return this.ParentSkill.Id - other.ParentSkill.Id;
        }

        if (this.Level == other.Level || other is not CustomProfession otherProfession)
        {
            return this.Id - other.Id;
        }

        return this.Level switch
        {
            5 when other.GetRootProfession is CustomProfession otherRoot => this == otherRoot ? -1 : this.Id - otherRoot.Id,
            10 when ((IProfession)this).GetRootProfession is CustomProfession thisRoot => thisRoot == otherProfession
                ? 1
                : thisRoot.Id - otherProfession.Id,
            _ => this.Id - other.Id,
        };
    }

    /// <inheritdoc />
    public bool Equals(IProfession? other)
    {
        return this.Id == other?.Id;
    }

    /// <summary>Gets the <see cref="SCProfession"/> equivalent to this <see cref="CustomProfession"/>.</summary>
    /// <returns>The equivalent <see cref="SCProfession"/>.</returns>
    public SCProfession ToSpaceCore()
    {
        return this._scProfession;
    }
}
