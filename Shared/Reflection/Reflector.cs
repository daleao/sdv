namespace DaLion.Shared.Reflection;

#region using directives

using DaLion.Shared.Caching;
using DaLion.Shared.Extensions.Reflection;

#endregion using directives

/// <summary>Provides delegates to inaccessible code.</summary>
/// <remarks>Based on SMAPI's <see href="https://github.com/Pathoschild/SMAPI/blob/develop/src/SMAPI/Modules/Reflection/Reflector.cs">Reflector</see> class by Pathoschild.</remarks>
public static class Reflector
{
    private static readonly IntervalMemoryCache<string, Delegate> _delegateCache = new();
    private static readonly IntervalMemoryCache<string, Type> _typeCache = new();

    /// <summary>Starts a new cache interval, clearing stale reflection lookups.</summary>
    public static void NewCacheInterval()
    {
        _delegateCache.StartNewInterval();
    }

    #region fields

    /// <summary>Gets a delegate which returns the value of an instance field.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="type">The type which has the field.</param>
    /// <param name="name">The field name.</param>
    /// <returns>A delegate for getting the field's value.</returns>
    public static Func<TInstance, TField> GetUnboundFieldGetter<TInstance, TField>(Type type, string name)
    {
        return GetCachedDelegate(
            "fg",
            type,
            name,
            false,
            () => type.RequireField(name).CompileUnboundFieldGetterDelegate<TInstance, TField>());
    }

    /// <summary>Gets a delegate which returns the value of an instance field.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="instance">The instance which has the field.</param>
    /// <param name="name">The field name.</param>
    /// <returns>A delegate for getting the field's value.</returns>
    public static Func<TInstance, TField> GetUnboundFieldGetter<TInstance, TField>(object instance, string name)
    {
        return GetUnboundFieldGetter<TInstance, TField>(instance.GetType(), name);
    }

    /// <summary>Gets a delegate which returns the value of an instance field.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="typeName">The name of the type which has the field.</param>
    /// <param name="fieldName">The field name.</param>
    /// <returns>A delegate for getting the field's value.</returns>
    public static Func<TInstance, TField> GetUnboundFieldGetter<TInstance, TField>(string typeName, string fieldName)
    {
        return GetUnboundFieldGetter<TInstance, TField>(typeName.ToType(), fieldName);
    }

    /// <summary>Gets a delegate which returns the value of an instance field.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="name">The field name.</param>
    /// <returns>A delegate for getting the field's value.</returns>
    /// <remarks>This overload requires that the target field be declared within the hierarchy of <typeparamref name="TInstance"/>.</remarks>
    public static Func<TInstance, TField> GetUnboundFieldGetter<TInstance, TField>(string name)
    {
        var type = typeof(TInstance);
        return GetCachedDelegate(
            "fg",
            type,
            name,
            false,
            () => type.RequireField(name).CompileUnboundFieldGetterDelegate<TInstance, TField>());
    }

    /// <summary>Gets a delegate which returns the value of a static field.</summary>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="type">The type which declares the field.</param>
    /// <param name="name">The field name.</param>
    /// <returns>A delegate for getting the field's value.</returns>
    public static Func<TField> GetStaticFieldGetter<TField>(Type type, string name)
    {
        return GetCachedDelegate(
            "fg",
            type,
            name,
            true,
            () => type.RequireField(name).CompileStaticFieldGetterDelegate<TField>());
    }

    /// <summary>Gets a delegate which returns the value of a static field.</summary>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="typeName">The name of the type which declares the field.</param>
    /// <param name="fieldName">The field name.</param>
    /// <returns>A delegate for getting the field's value.</returns>
    public static Func<TField> GetStaticFieldGetter<TField>(string typeName, string fieldName)
    {
        var type = _typeCache.GetOrSet(typeName, typeName.ToType);
        return GetStaticFieldGetter<TField>(type, fieldName);
    }

    /// <summary>Gets a delegate which sets the value of an instance field.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="type">The type which has the field.</param>
    /// <param name="name">The field name.</param>
    /// <returns>A delegate for setting the field's value.</returns>
    public static Action<TInstance, TField> GetUnboundFieldSetter<TInstance, TField>(Type type, string name)
    {
        return GetCachedDelegate(
            "fs",
            type,
            name,
            false,
            () => type.RequireField(name).CompileUnboundFieldSetterDelegate<TInstance, TField>());
    }

    /// <summary>Gets a delegate which sets the value of an instance field.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="instance">The instance which has the field.</param>
    /// <param name="name">The field name.</param>
    /// <returns>A delegate for setting the field's value.</returns>
    public static Action<TInstance, TField> GetUnboundFieldSetter<TInstance, TField>(object instance, string name)
    {
        return GetUnboundFieldSetter<TInstance, TField>(instance.GetType(), name);
    }

    /// <summary>Gets a delegate which sets the value of an instance field.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="typeName">The name of the type which has the field.</param>
    /// <param name="fieldName">The field name.</param>
    /// <returns>A delegate for setting the field's value.</returns>
    public static Action<TInstance, TField> GetUnboundFieldSetter<TInstance, TField>(string typeName, string fieldName)
    {
        return GetUnboundFieldSetter<TInstance, TField>(typeName.ToType(), fieldName);
    }

    /// <summary>Gets a delegate which sets the value of an instance field.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="name">The field name.</param>
    /// <returns>A delegate for setting the field's value.</returns>
    /// <remarks>This overload requires that the target field be declared within the hierarchy of <typeparamref name="TInstance"/>.</remarks>
    public static Action<TInstance, TField> GetUnboundFieldSetter<TInstance, TField>(string name)
    {
        var type = typeof(TInstance);
        return GetCachedDelegate(
            "fs",
            type,
            name,
            false,
            () => type.RequireField(name).CompileUnboundFieldSetterDelegate<TInstance, TField>());
    }

    /// <summary>Gets a delegate which sets the value of a static field.</summary>
    /// <typeparam name="TField">The type that will be returned by the delegate.</typeparam>
    /// <param name="type">The type which declares the field.</param>
    /// <param name="name">The field name.</param>
    /// <returns>A delegate for setting the field's value.</returns>
    public static Action<TField> GetStaticFieldSetter<TField>(Type type, string name)
    {
        return GetCachedDelegate(
            "fs",
            type,
            name,
            true,
            () => type.RequireField(name).CompileStaticFieldSetterDelegate<TField>());
    }

    /// <summary>Gets a delegate which sets the value of a static field.</summary>
    /// <typeparam name="TField">The type that will be returned by the delegate.</typeparam>
    /// <param name="typeName">The name of the type which declares the field.</param>
    /// <param name="fieldName">The field name.</param>
    /// <returns>A delegate for setting the field's value.</returns>
    public static Action<TField> GetStaticFieldSetter<TField>(string typeName, string fieldName)
    {
        var type = _typeCache.GetOrSet(typeName, typeName.ToType);
        return GetStaticFieldSetter<TField>(type, fieldName);
    }

    #endregion fields

    #region properties

    /// <summary>Gets a delegate which returns the value of an instance property.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="type">The type which has the property.</param>
    /// <param name="name">The property name.</param>
    /// <returns>A delegate to the property getter.</returns>
    public static Func<TInstance, TProperty> GetUnboundPropertyGetter<TInstance, TProperty>(Type type, string name)
    {
        return GetCachedDelegate(
            "pg",
            type,
            name,
            false,
            () => type.RequirePropertyGetter(name).CompileUnboundDelegate<Func<TInstance, TProperty>>());
    }

    /// <summary>Gets a delegate which returns the value of an instance property.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="instance">The instance which has the property.</param>
    /// <param name="name">The property name.</param>
    /// <returns>A delegate to the property getter.</returns>
    public static Func<TInstance, TProperty> GetUnboundPropertyGetter<TInstance, TProperty>(object instance, string name)
    {
        return GetUnboundPropertyGetter<TInstance, TProperty>(instance.GetType(), name);
    }

    /// <summary>Gets a delegate which returns the value of an instance property.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="typeName">The name of the type which has the property.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>A delegate to the property getter.</returns>
    public static Func<TInstance, TProperty> GetUnboundPropertyGetter<TInstance, TProperty>(string typeName, string propertyName)
    {
        return GetUnboundPropertyGetter<TInstance, TProperty>(typeName.ToType(), propertyName);
    }

    /// <summary>Gets a delegate which returns the value of an instance property.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="name">The property name.</param>
    /// <returns>A delegate to the property getter.</returns>
    /// <remarks>This overload requires that the target field be declared within the hierarchy of <typeparamref name="TInstance"/>.</remarks>
    public static Func<TInstance, TProperty> GetUnboundPropertyGetter<TInstance, TProperty>(string name)
    {
        var type = typeof(TInstance);
        return GetCachedDelegate(
            "pg",
            type,
            name,
            false,
            () => type.RequirePropertyGetter(name).CompileUnboundDelegate<Func<TInstance, TProperty>>());
    }

    /// <summary>Gets a delegate which returns the value of a static property.</summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="type">The type which declares the property.</param>
    /// <param name="name">The property name.</param>
    /// <returns>A delegate to the property getter.</returns>
    public static Func<TProperty> GetStaticPropertyGetter<TProperty>(Type type, string name)
    {
        return GetCachedDelegate(
            "pg",
            type,
            name,
            true,
            () => type.RequirePropertyGetter(name).CompileStaticDelegate<Func<TProperty>>());
    }

    /// <summary>Gets a delegate which returns the value of a static property.</summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="typeName">The name of the type which declares the property.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>A delegate to the property getter.</returns>
    public static Func<TProperty> GetStaticPropertyGetter<TProperty>(string typeName, string propertyName)
    {
        var type = _typeCache.GetOrSet(typeName, typeName.ToType);
        return GetStaticPropertyGetter<TProperty>(type, propertyName);
    }

    /// <summary>Gets a delegate which sets the value of an instance property.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="type">The type which has the property.</param>
    /// <param name="name">The property name.</param>
    /// <returns>A delegate to the property setter.</returns>
    public static Action<TInstance, TProperty> GetUnboundPropertySetter<TInstance, TProperty>(Type type, string name)
    {
        return GetCachedDelegate(
            "ps",
            type,
            name,
            false,
            () => type.RequirePropertySetter(name).CompileUnboundDelegate<Action<TInstance, TProperty>>());
    }

    /// <summary>Gets a delegate which sets the value of an instance property.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="instance">The instance which has the property.</param>
    /// <param name="name">The property name.</param>
    /// <returns>A delegate to the property setter.</returns>
    public static Action<TInstance, TProperty> GetUnboundPropertySetter<TInstance, TProperty>(object instance, string name)
    {
        return GetUnboundPropertySetter<TInstance, TProperty>(instance.GetType(), name);
    }

    /// <summary>Gets a delegate which sets the value of an instance property.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="typeName">The name of the type which has the property.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>A delegate to the property setter.</returns>
    public static Action<TInstance, TProperty> GetUnboundPropertySetter<TInstance, TProperty>(string typeName, string propertyName)
    {
        return GetUnboundPropertySetter<TInstance, TProperty>(typeName.ToType(), propertyName);
    }

    /// <summary>Gets a delegate which sets the value of an instance property.</summary>
    /// <typeparam name="TInstance">The type of the instance that the delegate will be invoked on.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="name">The property name.</param>
    /// <returns>A delegate to the property setter.</returns>
    /// <remarks>This overload requires that the target field be declared within the hierarchy of <typeparamref name="TInstance"/>.</remarks>
    public static Action<TInstance, TProperty> GetUnboundPropertySetter<TInstance, TProperty>(string name)
    {
        var type = typeof(TInstance);
        return GetCachedDelegate(
            "ps",
            type,
            name,
            false,
            () => type.RequirePropertySetter(name).CompileUnboundDelegate<Action<TInstance, TProperty>>());
    }

    /// <summary>Gets a delegate which sets the value of a static property.</summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="type">The type which declares the property.</param>
    /// <param name="name">The property name.</param>
    /// <returns>A delegate to the property setter.</returns>
    public static Action<TProperty> GetStaticPropertySetter<TProperty>(Type type, string name)
    {
        return GetCachedDelegate(
            "ps",
            type,
            name,
            true,
            () => type.RequirePropertySetter(name).CompileStaticDelegate<Action<TProperty>>());
    }

    /// <summary>Gets a delegate which sets the value of a static property.</summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="typeName">The name of the type which declares the property.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>A delegate to the property setter.</returns>
    public static Action<TProperty> GetStaticPropertySetter<TProperty>(string typeName, string propertyName)
    {
        var type = _typeCache.GetOrSet(typeName, typeName.ToType);
        return GetStaticPropertySetter<TProperty>(type, propertyName);
    }

    #endregion properties

    #region methods

    /// <summary>Gets a delegate which calls an instance method.</summary>
    /// <typeparam name="TDelegate">
    ///     A delegate type which mirrors the desired method signature and accepts the target
    ///     instance type as the first parameter.</typeparam>
    /// <param name="type">The type which has the method.</param>
    /// <param name="name">The method name.</param>
    /// <returns>A delegate to the method.</returns>
    public static TDelegate GetUnboundMethodDelegate<TDelegate>(Type type, string name)
        where TDelegate : Delegate
    {
        return GetCachedDelegate(
            "m",
            type,
            name,
            false,
            () => type.RequireMethod(name).CompileUnboundDelegate<TDelegate>());
    }

    /// <summary>Gets a delegate which calls an instance method.</summary>
    /// <typeparam name="TDelegate">
    ///     A delegate type which mirrors the desired method signature and accepts the target
    ///     instance type as the first parameter.</typeparam>
    /// <param name="instance">The instance which has the method.</param>
    /// <param name="name">The method name.</param>
    /// <returns>A delegate to the method.</returns>
    public static TDelegate GetUnboundMethodDelegate<TDelegate>(object instance, string name)
        where TDelegate : Delegate
    {
        return GetUnboundMethodDelegate<TDelegate>(instance.GetType(), name);
    }

    /// <summary>Gets a delegate which calls an instance method.</summary>
    /// <typeparam name="TDelegate">
    ///     A delegate type which mirrors the desired method signature and accepts the target
    ///     instance type as the first parameter.</typeparam>
    /// <param name="typeName">The name of the type which has the method.</param>
    /// <param name="methodName">The method name.</param>
    /// <returns>A delegate to the method.</returns>
    public static TDelegate GetUnboundMethodDelegate<TDelegate>(string typeName, string methodName)
        where TDelegate : Delegate
    {
        return GetUnboundMethodDelegate<TDelegate>(typeName.ToType(), methodName);
    }

    /// <summary>Gets a delegate which calls a static method.</summary>
    /// <typeparam name="TDelegate">A delegate type which mirrors the desired method signature.</typeparam>
    /// <param name="type">The type which declares the method.</param>
    /// <param name="name">The method name.</param>
    /// <returns>A delegate to the method.</returns>
    public static TDelegate GetStaticMethodDelegate<TDelegate>(Type type, string name)
        where TDelegate : Delegate
    {
        return GetCachedDelegate(
            "m",
            type,
            name,
            true,
            () => type.RequireMethod(name).CompileStaticDelegate<TDelegate>());
    }

    /// <summary>Gets a delegate which calls a static method.</summary>
    /// <typeparam name="TDelegate">A delegate type which mirrors the desired method signature.</typeparam>
    /// <param name="typeName">The name of the type which declares the method.</param>
    /// <param name="methodName">The method name.</param>
    /// <returns>A delegate to the method.</returns>
    public static TDelegate GetStaticMethodDelegate<TDelegate>(string typeName, string methodName)
        where TDelegate : Delegate
    {
        var type = _typeCache.GetOrSet(typeName, typeName.ToType);
        return GetStaticMethodDelegate<TDelegate>(type, methodName);
    }

    #endregion methods

    /// <summary>Retrieves an existing delegate instance from the cache, or caches a new instance.</summary>
    /// <typeparam name="TDelegate">The expected <see cref="Delegate"/> type.</typeparam>
    /// <param name="prefix">A letter or letters representing the member type (like 'm' for method).</param>
    /// <param name="type">The declaring type of the reflected member.</param>
    /// <param name="name">The member name.</param>
    /// <param name="isStatic">Whether the member is static.</param>
    /// <param name="fetch">Fetches a new value to cache.</param>
    /// <returns>The cached delegated.</returns>
    private static TDelegate GetCachedDelegate<TDelegate>(string prefix, Type type, string name, bool isStatic, Func<TDelegate> fetch)
        where TDelegate : Delegate
    {
        var key = $"{prefix}{(isStatic ? 's' : 'i')}{type.FullName}:{name}";
        return (TDelegate)_delegateCache.GetOrSet(key, fetch);
    }
}
