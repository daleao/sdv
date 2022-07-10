namespace DaLion.Common.Extensions.Reflection;

#region using directives

using HarmonyLib;
using System;

#endregion using directives

/// <summary>Extensions for the <see cref="string"/> primitive type.</summary>
public static class StringExtensions
{
    /// <summary>Get a type by name and assert that it was found.</summary>
    public static Type ToType(this string name) =>
        AccessTools.TypeByName(name) ?? throw new($"Couldn't find type named {name}.");
}