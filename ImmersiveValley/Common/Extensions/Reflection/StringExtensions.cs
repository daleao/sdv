namespace DaLion.Common.Extensions.Reflection;

#region using directives

using System;
using CommunityToolkit.Diagnostics;
using DaLion.Common.Exceptions;
using HarmonyLib;

#endregion using directives

/// <summary>Extensions for the <see cref="string"/> primitive type.</summary>
public static class StringExtensions
{
    /// <summary>Gets a type in the assembly by <paramref name="name"/> and asserts that it was found.</summary>
    /// <param name="name">The name of some type in any executing assembly.</param>
    /// <returns>The corresponding <see cref="Type"/>, if found.</returns>
    /// <exception cref="MissingTypeException">If the requested type is not found.</exception>
    public static Type ToType(this string name)
    {
        return AccessTools.TypeByName(name) ?? ThrowHelperExtensions.ThrowMissingTypeException(name);
    }
}
