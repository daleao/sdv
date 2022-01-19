namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;

#endregion using directives

public static class StringExtensions
{
    /// <summary>Get the index of a given profession by name.</summary>
    public static int ToProfessionIndex(this string professionName)
    {
        if (Enum.TryParse<Profession>(professionName, out var profession)) return (int)profession;
        throw new ArgumentException($"Profession {professionName} does not exist.");
    }

}