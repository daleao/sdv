namespace DaLion.Redux.Framework.Professions;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>Represents a profession tied to a mod-provided <see cref="ISkill"/>.</summary>
/// <remarks>This applies to both SpaceCore <see cref="SCSkill"/>s and the special-case <see cref="LuckSkill"/>.</remarks>
public sealed class SCProfession : IProfession
{
    private readonly Func<string> _titleGetter;

    private readonly Func<string> _descriptionGetter;

    private Func<string> _prestigeDescriptionGetter;

    /// <summary>Initializes a new instance of the <see cref="SCProfession"/> class.</summary>
    /// <param name="id">The integer id used in-game to track professions acquired by the player.</param>
    /// <param name="stringId">The string used by SpaceCore to uniquely identify this profession.</param>
    /// <param name="getTitle">A function for getting the localized in-game title of this profession.</param>
    /// <param name="getDescription">A function for getting the localized in-game description of this profession.</param>
    /// <param name="getPrestigeDescription">A function for getting the localized in-game description of the prestiged version of this profession.</param>
    /// <param name="level">The level at which this profession is offered.</param>
    /// <param name="skill">The <see cref="ISkill"/> to which this profession belongs.</param>
    internal SCProfession(
        int id,
        string stringId,
        Func<string> getTitle,
        Func<string> getDescription,
        Func<string>? getPrestigeDescription,
        int level,
        ISkill skill)
    {
        this.Id = id;
        this.StringId = stringId;
        this.Level = level;
        this.Skill = skill;
        this._titleGetter = getTitle;
        this._descriptionGetter = getDescription;
        this._prestigeDescriptionGetter = getPrestigeDescription ?? getDescription;
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
    internal static Dictionary<int, SCProfession> LoadedProfessions { get; } = new();

    /// <inheritdoc />
    public string GetDescription(bool prestiged = false)
    {
        return prestiged
            ? this._prestigeDescriptionGetter.Invoke()
            : this._descriptionGetter.Invoke();
    }

    /// <summary>Sets the prestige description getter for this profession.</summary>
    /// <param name="getPrestigeDescription">A delegate which returns the localized prestige description for this profession.</param>
    internal void SetPrestigeDescriptionGetter(Func<string> getPrestigeDescription)
    {
        this._prestigeDescriptionGetter = getPrestigeDescription;
    }
}
