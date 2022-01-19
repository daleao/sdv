namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;

#endregion using directives

public static class IntExtensions
{
    /// <summary>Get the name of a given profession by index.</summary>
    public static string ToProfessionName(this int professionIndex)
    {
        if (Enum.TryParse<Profession>(professionIndex.ToString(), out var profession)) return profession.ToString();
        throw new ArgumentException($"Profession {professionIndex} does not exist.");
    }
}