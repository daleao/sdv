#nullable enable
namespace DaLion.Common.Extensions.Reflection;

#region using directives

using System;
using HarmonyLib;

#endregion using directives

/// <summary>Extensions for the <see cref="string"/> primitive type.</summary>
public static class StringExtensions
{
    /// <summary>Get a type by name and assert that it was found.</summary>
    public static Type ToType(this string name)
    {
        return AccessTools.TypeByName(name) ?? throw new($"Cannot find type named {name}.");
    }
}