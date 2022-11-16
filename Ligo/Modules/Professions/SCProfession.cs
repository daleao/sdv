namespace DaLion.Ligo.Modules.Professions;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Classes;

#endregion using directives

/// <summary>Represents a profession tied to a mod-provided <see cref="ISkill"/>.</summary>
/// <remarks>This applies to both SpaceCore <see cref="SCSkill"/>s and the special-case <see cref="LuckSkill"/>.</remarks>
public sealed class SCProfession : IProfession
{
    private static readonly BiMap<SCProfession, SpaceCore.Skills.Skill.Profession> LigoToSpaceCore = new();

    private readonly Func<string> _titleGetter;
    private readonly Func<string> _descriptionGetter;

    /// <summary>Initializes a new instance of the <see cref="SCProfession"/> class.</summary>
    /// <param name="id">The integer id used in-game to track professions acquired by the player.</param>
    /// <param name="stringId">The string used by SpaceCore to uniquely identify this profession.</param>
    /// <param name="getTitle">A function for getting the localized in-game title of this profession.</param>
    /// <param name="getDescription">A function for getting the localized in-game description of this profession.</param>
    /// <param name="level">The level at which this profession is offered.</param>
    /// <param name="skill">The <see cref="ISkill"/> to which this profession belongs.</param>
    internal SCProfession(
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

    /// <summary>Initializes a new instance of the <see cref="SCProfession"/> class from an equivalent <see cref="SpaceCore.Skills.Skill.Profession"/>.</summary>
    /// <param name="profession">The equivalent <see cref="SpaceCore.Skills.Skill.Profession"/>.</param>
    /// <param name="level">The level at which this profession is offered.</param>
    /// <param name="skill">The <see cref="ISkill"/> to which this profession belongs.</param>
    internal SCProfession(SpaceCore.Skills.Skill.Profession profession, int level, ISkill skill)
    {
        this.Id = profession.GetVanillaId();
        this.StringId = profession.Id;
        this.Level = level;
        this.Skill = skill;
        this._titleGetter = profession.GetName;
        this._descriptionGetter = profession.GetDescription;
        LigoToSpaceCore.TryAdd(this, profession);
    }

    /// <inheritdoc />
    public int Id { get; }

    /// <inheritdoc />
    public string StringId { get; }

    /// <inheritdoc />
    public int Level { get; }

    /// <inheritdoc />
    public ISkill Skill { get; }

    /// <inheritdoc />
    public string Title => this._titleGetter.Invoke();

    /// <summary>Gets professions for loaded <see cref="SCSkill"/>s.</summary>
    internal static Dictionary<int, SCProfession> Loaded { get; } = new();

    /// <summary>Gets the <see cref="SCProfession"/> equivalent to the specified <see cref="SpaceCore.Skills.Skill.Profession"/>.</summary>
    /// <param name="profession">The <see cref="SpaceCore.Skills.Skill.Profession"/>.</param>
    /// <returns>The equivalent <see cref="SCProfession"/>.</returns>
    public static SCProfession? FromSpaceCore(SpaceCore.Skills.Skill.Profession profession)
    {
        return LigoToSpaceCore.TryGetReverse(profession, out var scProfession) ? scProfession : null;
    }

    /// <summary>Gets the <see cref="SpaceCore.Skills.Skill.Profession"/> equivalent to this <see cref="SCProfession"/>.</summary>
    /// <returns>The equivalent <see cref="SpaceCore.Skills.Skill.Profession"/>.</returns>
    public SpaceCore.Skills.Skill.Profession? ToSpaceCore()
    {
        return LigoToSpaceCore.TryGetForward(this, out var profession) ? profession : null;
    }

    /// <inheritdoc />
    public string GetDescription(bool prestiged = false)
    {
        return this._descriptionGetter.Invoke();
    }
}
