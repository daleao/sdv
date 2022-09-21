namespace DaLion.Stardew.Professions.Framework;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>Represents a custom profession tied to a mod-provided <see cref="ISkill"/>.</summary>
/// <param name="StringId">The string that uniquely identifies this profession.</param>
/// <param name="DisplayName">The localized in-game name of this profession.</param>
/// <param name="Description">The localized in-game description of this profession.</param>
/// <param name="Id">The integer id used in-game to track professions acquired by the player.</param>
/// <param name="Level">The level at which this profession is offered.</param>
/// <param name="Skill">The <see cref="ISkill"/> to which this profession belongs.</param>
/// <remarks>This applies to both SpaceCore <see cref="CustomSkill"/>s and the special-case <see cref="LuckSkill"/>.</remarks>
public record CustomProfession(string StringId, string DisplayName, string Description, int Id, int Level,
    ISkill Skill) : IProfession
{
    internal static Dictionary<int, CustomProfession> LoadedProfessions { get; set; } = new();

    /// <inheritdoc />
    public string GetDescription(bool prestiged = false)
    {
        return this.Description;
    }
}
