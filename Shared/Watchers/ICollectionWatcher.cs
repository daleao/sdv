namespace DaLion.Shared.Watchers;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>A watcher which tracks changes to a <see cref="ICollection{T}"/>.</summary>
/// <typeparam name="TValue">The collection value type.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal interface ICollectionWatcher<out TValue> : IWatcher
{
    /// <summary>Gets the values added since the last reset.</summary>
    IEnumerable<TValue> Added { get; }

    /// <summary>Gets the values removed since the last reset.</summary>
    IEnumerable<TValue> Removed { get; }
}
