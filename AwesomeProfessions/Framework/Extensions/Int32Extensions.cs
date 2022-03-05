namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;
using StardewModdingAPI.Enums;

#endregion using directives

internal static class Int32Extensions
{
    /// <summary>Get the name of a given profession by index.</summary>
    internal static string ToProfessionName(this int professionIndex)
    {
        if (Enum.TryParse<Profession>(professionIndex.ToString(), out var profession)) return profession.ToString();
        throw new ArgumentException($"Profession {professionIndex} does not exist.");
    }

    /// <summary>Get the name of a given profession by index.</summary>
    internal static SkillType GetCorrespondingSkill(this int professionIndex)
    {
        return (SkillType) (professionIndex / 6);
    }

    /// <summary>Whether a given object index corresponds to algae or seaweed.</summary>
    internal static bool IsAlgae(this int objectIndex)
    {
        return objectIndex is 152 or 153 or 157;
    }

    /// <summary>Whether a given object index corresponds to trash.</summary>
    internal static bool IsTrash(this int objectIndex)
    {
        return objectIndex is > 166 and < 173;
    }
}