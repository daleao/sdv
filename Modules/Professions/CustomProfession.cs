namespace DaLion.Overhaul.Modules.Professions;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Shared.Classes;
using SCProfession = SpaceCore.Skills.Skill.Profession;

#endregion using directives

/// <summary>Represents a profession tied to a mod-provided <see cref="ISkill"/>.</summary>
/// <remarks>This applies to both SpaceCore <see cref="CustomSkill"/>s and the special-case <see cref="LuckSkill"/>.</remarks>
public sealed class CustomProfession : IProfession
{
    private static readonly BiMap<CustomProfession, SCProfession> SpaceCoreMap = new();

    private readonly Func<string> _titleGetter;
    private readonly Func<string> _descriptionGetter;

    /// <summary>Initializes a new instance of the <see cref="CustomProfession"/> class.</summary>
    /// <param name="id">The integer id used in-game to track professions acquired by the player.</param>
    /// <param name="stringId">The string used by SpaceCore to uniquely identify this profession.</param>
    /// <param name="getTitle">A function for getting the localized in-game title of this profession.</param>
    /// <param name="getDescription">A function for getting the localized in-game description of this profession.</param>
    /// <param name="level">The level at which this profession is offered.</param>
    /// <param name="skill">The <see cref="ISkill"/> to which this profession belongs.</param>
    internal CustomProfession(
        int id,
        string stringId,
        Func<string> getTitle,
        Func<string> getDescription,
        int level,
        ISkill skill)
    {
        this.Id = id;
        this.StringId = stringId;
        this.Level = level;
        this.Skill = skill;
        this._titleGetter = getTitle;
        this._descriptionGetter = getDescription;
    }

    /// <summary>Initializes a new instance of the <see cref="CustomProfession"/> class from an equivalent <see cref="SCProfession"/>.</summary>
    /// <param name="scProfession">The equivalent <see cref="SCProfession"/>.</param>
    /// <param name="level">The level at which this profession is offered.</param>
    /// <param name="skill">The <see cref="ISkill"/> to which this profession belongs.</param>
    internal CustomProfession(SCProfession scProfession, int level, ISkill skill)
    {
        this.Id = scProfession.GetVanillaId();
        this.StringId = scProfession.Id;
        this.Level = level;
        this.Skill = skill;
        this._titleGetter = scProfession.GetName;
        this._descriptionGetter = scProfession.GetDescription;
        SpaceCoreMap.TryAdd(this, scProfession);
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
    public ISkill Skill { get; }

    /// <summary>Gets professions for loaded <see cref="CustomSkill"/>s.</summary>
    internal static Dictionary<int, CustomProfession> Loaded { get; } = new();

    /// <summary>Enumerates all the ids of loaded <see cref="CustomProfession"/> instances.</summary>
    /// <param name="prestige">Whether to enumerate prestige professions instead.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all loaded SpaceCore profession indices.</returns>
    public static IEnumerable<int> GetAllIds(bool prestige = false)
    {
        return List.Select(p => prestige ? p.Id + 100 : p.Id);
    }

    /// <summary>Gets the <see cref="CustomProfession"/> equivalent to the specified <see cref="SpaceCore.Skills.Skill.Profession"/>.</summary>
    /// <param name="scProfession">The <see cref="SCProfession"/>.</param>
    /// <returns>The equivalent <see cref="CustomProfession"/>.</returns>
    public static CustomProfession? FromSpaceCore(SCProfession scProfession)
    {
        return SpaceCoreMap.TryGetReverse(scProfession, out var customProfession) ? customProfession : null;
    }

    /// <summary>Gets the <see cref="SCProfession"/> equivalent to this <see cref="CustomProfession"/>.</summary>
    /// <returns>The equivalent <see cref="SCProfession"/>.</returns>
    public SCProfession? ToSpaceCore()
    {
        return SpaceCoreMap.TryGetForward(this, out var scProfession) ? scProfession : null;
    }
}
