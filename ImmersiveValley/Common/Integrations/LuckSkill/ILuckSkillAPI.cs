namespace DaLion.Common.Integrations;

#region using directives

using System.Collections.Generic;

#endregion using directives

public interface ILuckSkillAPI
{
	IDictionary<int, IProfession> GetProfessions();

    public interface IProfession
    {
        int Id { get; }

        string DefaultName { get; }

        string Name { get; }

        string DefaultDescription { get; }

        string Description { get; }
    }
}