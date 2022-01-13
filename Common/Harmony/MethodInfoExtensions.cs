using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace DaLion.Stardew.Common.Harmony;

public static class MethodInfoExtensions
{
    /// <summary>Construct a <see cref="HarmonyMethod" /> instance from a <see cref="MethodInfo" /> object.</summary>
    /// <returns>
    ///     Returns a new <see cref="HarmonyMethod" /> instance if <paramref name="method" /> is not null, or <c>null</c>
    ///     otherwise.
    /// </returns>
    [CanBeNull]
    public static HarmonyMethod ToHarmonyMethod(this MethodInfo method)
    {
        return method is null ? null : new HarmonyMethod(method);
    }
}