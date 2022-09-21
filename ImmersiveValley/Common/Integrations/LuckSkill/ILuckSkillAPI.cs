#pragma warning disable CS1591
namespace DaLion.Common.Integrations.LuckSkill;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The API provided by Luck Skill mod.</summary>
public interface ILuckSkillApi
{
    public interface IProfession
    {
        int Id { get; }

        string DefaultName { get; }

        string Name { get; }

        string DefaultDescription { get; }

        string Description { get; }
    }

    IDictionary<int, IProfession> GetProfessions();
}
