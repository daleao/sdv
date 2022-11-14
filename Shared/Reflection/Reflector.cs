namespace DaLion.Shared.Reflection;

#region using directives

using DaLion.Shared.Caching;
using DaLion.Shared.Extensions.Reflection;

#endregion using directives

/// <summary>Provides delegates to inaccessible code.</summary>
/// <remarks>Based on SMAPI's <see href="https://github.com/Pathoschild/SMAPI/blob/develop/src/SMAPI/Modules/Reflection/Reflector.cs">Reflector</see> class by Pathoschild.</remarks>
internal sealed class Reflector
{
    private readonly IntervalMemoryCache<string, Delegate> _delegateCache = new();
    private readonly IntervalMemoryCache<string, Type> _typeCache = new();

    /// <summary>Starts a new cache interval, clearing stale reflection lookups.</summary>
    public void NewCacheInterval()
    {
        this._delegateCache.StartNewInterval();
    }

    #region fields

    /// <summary>Gets a delegate which returns the value of an instance field.</summary>
    /// <typeparam name="TInstance">The type of the instance which declares the field.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="instance">The instance which has the field.</param>
    /// <param name="name">The field name.</param>
    /// <returns>Returns the field getter delegate.</returns>
    public Func<TInstance, TField> GetUnboundFieldGetter<TInstance, TField>(object instance, string name)
    {
        var type = instance.GetType();
        return this.GetCachedDelegate(
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
    /// <returns>Returns the field getter delegate.</returns>
    public Func<TField> GetStaticFieldGetter<TField>(Type type, string name)
    {
        return this.GetCachedDelegate(
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
    /// <returns>Returns the field getter delegate.</returns>
    public Func<TField> GetStaticFieldGetter<TField>(string typeName, string fieldName)
    {
        var type = this._typeCache.GetOrSet(typeName, typeName.ToType);
        return this.GetStaticFieldGetter<TField>(type, fieldName);
    }

    /// <summary>Gets a delegate which sets the value of an instance field.</summary>
    /// <typeparam name="TInstance">The type of the instance which declares the field.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="instance">The instance which has the field.</param>
    /// <param name="name">The field name.</param>
    /// <returns>Returns the field setter delegate.</returns>
    public Action<TInstance, TField> GetUnboundFieldSetter<TInstance, TField>(object instance, string name)
    {
        var type = instance.GetType();
        return this.GetCachedDelegate(
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
    /// <returns>Returns the field setter delegate.</returns>
    public Action<TField> GetStaticFieldSetter<TField>(Type type, string name)
    {
        return this.GetCachedDelegate(
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
    /// <returns>Returns the field setter delegate.</returns>
    public Action<TField> GetStaticFieldSetter<TField>(string typeName, string fieldName)
    {
        var type = this._typeCache.GetOrSet(typeName, typeName.ToType);
        return this.GetStaticFieldSetter<TField>(type, fieldName);
    }

    #endregion fields

    #region properties

    /// <summary>Gets a delegate which returns the value of an instance property.</summary>
    /// <typeparam name="TInstance">The type of the instance which declares the property.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="instance">The instance which has the property.</param>
    /// <param name="name">The property name.</param>
    /// <returns>Returns the property getter delegate.</returns>
    public Func<TInstance, TProperty> GetUnboundPropertyGetter<TInstance, TProperty>(object instance, string name)
    {
        var type = instance.GetType();
        return this.GetCachedDelegate(
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
    /// <returns>Returns the property getter delegate.</returns>
    public Func<TProperty> GetStaticPropertyGetter<TProperty>(Type type, string name)
    {
        return this.GetCachedDelegate(
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
    /// <returns>Returns the property getter delegate.</returns>
    public Func<TProperty> GetStaticPropertyGetter<TProperty>(string typeName, string propertyName)
    {
        var type = this._typeCache.GetOrSet(typeName, typeName.ToType);
        return this.GetStaticPropertyGetter<TProperty>(type, propertyName);
    }

    /// <summary>Gets a delegate which sets the value of an instance property.</summary>
    /// <typeparam name="TInstance">The type of the instance which declares the property.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="instance">The instance which has the property.</param>
    /// <param name="name">The property name.</param>
    /// <returns>Returns the property setter delegate.</returns>
    public Action<TInstance, TProperty> GetUnboundPropertySetter<TInstance, TProperty>(object instance, string name)
    {
        var type = instance.GetType();
        return this.GetCachedDelegate(
            "ps",
            type,
            name,
            false,
            () => type.RequirePropertyGetter(name).CompileUnboundDelegate<Action<TInstance, TProperty>>());
    }

    /// <summary>Gets a delegate which sets the value of a static property.</summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="type">The type which declares the property.</param>
    /// <param name="name">The property name.</param>
    /// <returns>Returns the property setter delegate.</returns>
    public Action<TProperty> GetStaticPropertySetter<TProperty>(Type type, string name)
    {
        return this.GetCachedDelegate(
            "ps",
            type,
            name,
            true,
            () => type.RequirePropertyGetter(name).CompileStaticDelegate<Action<TProperty>>());
    }

    /// <summary>Gets a delegate which sets the value of a static property.</summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="typeName">The name of the type which declares the property.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>Returns the property setter delegate.</returns>
    public Action<TProperty> GetStaticPropertySetter<TProperty>(string typeName, string propertyName)
    {
        var type = this._typeCache.GetOrSet(typeName, typeName.ToType);
        return this.GetStaticPropertySetter<TProperty>(type, propertyName);
    }

    #endregion properties

    #region methods

    /// <summary>Gets a delegate which calls an instance method.</summary>
    /// <typeparam name="TDelegate">
    ///     A delegate type which mirrors the desired method signature and accepts the target
    ///     instance type as the first parameter.</typeparam>
    /// <param name="instance">The instance which has the method.</param>
    /// <param name="name">The method name.</param>
    /// <returns>Returns the method delegate.</returns>
    public TDelegate GetUnboundMethodDelegate<TDelegate>(object instance, string name)
        where TDelegate : Delegate
    {
        var type = instance.GetType();
        return this.GetCachedDelegate(
            "m",
            type,
            name,
            false,
            () => type.RequireMethod(name).CompileUnboundDelegate<TDelegate>());
    }

    /// <summary>Gets a delegate which calls a static method.</summary>
    /// <typeparam name="TDelegate">A delegate type which mirrors the desired method signature.</typeparam>
    /// <param name="type">The type which declares the method.</param>
    /// <param name="name">The method name.</param>
    /// <returns>Returns the method delegate.</returns>
    public TDelegate GetStaticMethodDelegate<TDelegate>(Type type, string name)
        where TDelegate : Delegate
    {
        return this.GetCachedDelegate(
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
    /// <returns>Returns the method delegate.</returns>
    public TDelegate GetStaticMethodDelegate<TDelegate>(string typeName, string methodName)
        where TDelegate : Delegate
    {
        var type = this._typeCache.GetOrSet(typeName, typeName.ToType);
        return this.GetStaticMethodDelegate<TDelegate>(type, methodName);
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
    private TDelegate GetCachedDelegate<TDelegate>(string prefix, Type type, string name, bool isStatic, Func<TDelegate> fetch)
        where TDelegate : Delegate
    {
        var key = $"{prefix}{(isStatic ? 's' : 'i')}{type.FullName}:{name}";
        return (TDelegate)this._delegateCache.GetOrSet(key, fetch);
    }
}
