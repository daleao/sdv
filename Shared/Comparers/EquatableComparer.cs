namespace DaLion.Shared.Comparers;

#region using directives

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#endregion using directives

/// <summary>Compares instances using <see cref="IEqualityComparer{T}.Equals(T,T)"/>.</summary>
/// <typeparam name="T">The value type.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal class EquatableComparer<T> : IEqualityComparer<T>
    where T : IEquatable<T>
{
    /// <summary>Determines whether the specified objects are equal.</summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns><see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(T? x, T? y)
    {
        return x == null
            ? y == null
            : x.Equals(y);
    }

    /// <summary>Gets a hash code for the specified <paramref name="object"/>.</summary>
    /// <param name="object">The value.</param>
    /// <returns>A hash code for <paramref name="object"/>.</returns>
    public int GetHashCode(T @object)
    {
        return RuntimeHelpers.GetHashCode(@object);
    }
}
