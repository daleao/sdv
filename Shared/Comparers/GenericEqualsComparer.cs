namespace DaLion.Shared.Comparers;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#endregion using directives

/// <summary>Compares values using their <see cref="object.Equals(object)"/> method. This should only be used when <see cref="EquatableComparer{T}"/> won't work, since this doesn't validate whether they're comparable.</summary>
/// <typeparam name="T">The value type.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal class GenericEqualsComparer<T> : IEqualityComparer<T>
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
