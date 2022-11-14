namespace DaLion.Shared.Watchers;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>A watcher which tracks changes to a dictionary.</summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal interface IDictionaryWatcher<TKey, TValue> : ICollectionWatcher<KeyValuePair<TKey, TValue>>
{
}
