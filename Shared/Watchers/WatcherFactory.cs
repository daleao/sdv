namespace DaLion.Shared.Watchers;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Comparers;
using Netcode;

#endregion using directives

/// <summary>Provides convenience wrappers for creating watchers.</summary>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal static class WatcherFactory
{
    /// <summary>Creates a watcher for a <see cref="IEquatable{T}"/> value.</summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="getValue">Get the current value.</param>
    /// <returns>An interface for a new instance of <see cref="ComparableValueWatcher{TValue}"/>.</returns>
    public static IValueWatcher<TValue> ForEquatable<TValue>(string name, Func<TValue> getValue)
        where TValue : IEquatable<TValue>
    {
        return new ComparableValueWatcher<TValue>(name, getValue, new EquatableComparer<TValue>());
    }

    /// <summary>Get a watcher which compares values using their <see cref="object.Equals(object)"/> method. This method should only be used when <see cref="ForEquatable{T}"/> won't work, since this doesn't validate whether they're comparable.</summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="getValue">Get the current value.</param>
    /// <returns>An interface for a new instance of <see cref="ComparableValueWatcher{TValue}"/>.</returns>
    public static IValueWatcher<TValue> ForGenericEquality<TValue>(string name, Func<TValue> getValue)
        where TValue : struct
    {
        return new ComparableValueWatcher<TValue>(name, getValue, new GenericEqualsComparer<TValue>());
    }

    /// <summary>Get a watcher which detects when an object reference changes.</summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="getValue">Get the current value.</param>
    /// <returns>An interface for a new instance of <see cref="ComparableValueWatcher{TValue}"/>.</returns>
    public static IValueWatcher<TValue> ForReference<TValue>(string name, Func<TValue> getValue)
        where TValue : class
    {
        return new ComparableValueWatcher<TValue>(name, getValue, new ObjectReferenceComparer<TValue>());
    }

    /// <summary>Creates a watcher for a <see cref="NetFieldBase{T,TSelf}"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TSelf">The net field instance type.</typeparam>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="field">The <see cref="NetFieldBase{T,TSelf}"/>.</param>
    /// <returns>An interface for a new instance of <see cref="NetFieldWatcher{TValue,TNetField}"/>.</returns>
    public static IValueWatcher<T> ForNetValue<T, TSelf>(string name, NetFieldBase<T, TSelf> field)
        where TSelf : NetFieldBase<T, TSelf>
    {
        return new NetFieldWatcher<T, TSelf>(name, field);
    }

    /// <summary>Creates a watcher for a <see cref="NetCollection{T}"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="collection">The <see cref="NetCollection{T}"/>.</param>
    /// <returns>An interface for a new instance of <see cref="NetCollectionWatcher{TValue}"/>.</returns>
    public static ICollectionWatcher<T> ForNetCollection<T>(string name, NetCollection<T> collection)
        where T : class, INetObject<INetSerializable>
    {
        return new NetCollectionWatcher<T>(name, collection);
    }

    /// <summary>Creates a watcher for a <see cref="NetList{T,TField}"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="collection">The <see cref="NetList{T,TField}"/>.</param>
    /// <returns>An interface for a new instance of <see cref="NetListWatcher{TValue}"/>.</returns>
    public static ICollectionWatcher<T> ForNetList<T>(string name, NetList<T, NetRef<T>> collection)
        where T : class, INetObject<INetSerializable>
    {
        return new NetListWatcher<T>(name, collection);
    }

    /// <summary>Creates a watcher for a <see cref="NetIntList"/>.</summary>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="collection">The <see cref="NetIntList"/>.</param>
    /// <returns>An interface for a new instance of <see cref="NetListWatcher{TValue}"/>.</returns>
    public static ICollectionWatcher<int> ForNetIntList(string name, NetIntList collection)
    {
        return new NetIntListWatcher(name, collection);
    }

    /// <summary>Creates a watcher for a <see cref="NetDictionary{TKey,TValue,TField,TSerialDict,TSelf}"/>.</summary>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <typeparam name="TField">The net type equivalent to <typeparamref name="TValue"/>.</typeparam>
    /// <typeparam name="TSerialDict">The serializable dictionary type that can store the keys and values.</typeparam>
    /// <typeparam name="TSelf">The net field instance type.</typeparam>
    /// <param name="field">The <see cref="NetDictionary{TKey,TValue,TField,TSerialDict,TSelf}"/>.</param>
    /// <returns>An interface for a new instance of <see cref="NetDictionaryWatcher{TKey,TValue,TField,TSerialDict,TSelf}"/>.</returns>
    public static NetDictionaryWatcher<TKey, TValue, TField, TSerialDict, TSelf> ForNetDictionary<TKey, TValue, TField, TSerialDict, TSelf>(string name, NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> field)
        where TKey : notnull
        where TField : class, INetObject<INetSerializable>, new()
        where TSerialDict : IDictionary<TKey, TValue>, new()
        where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>
    {
        return new NetDictionaryWatcher<TKey, TValue, TField, TSerialDict, TSelf>(name, field);
    }
}
