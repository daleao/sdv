namespace DaLion.Shared.Harmony;

#region using directives

using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Base implementation of a <see cref="Harmony"/> patch class targeting a single method.</summary>
public abstract class HarmonyPatcher : IHarmonyPatcher
{
    private bool _isApplied;

    /// <summary>Initializes a new instance of the <see cref="HarmonyPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    protected HarmonyPatcher(Harmonizer harmonizer)
    {
        this.Harmonizer = harmonizer;
        if (this.GetType().GetCustomAttribute<HarmonyPatch>() is { } patchAttribute)
        {
            this.Target = patchAttribute.info.declaringType?.RequireMethod(
                patchAttribute.info.methodName,
                patchAttribute.info.argumentTypes);
        }

        (this.Prefix, this.Postfix, this.Transpiler, this.Finalizer, this.Reverse) = this.GetHarmonyMethods();
    }

    /// <inheritdoc />
    public MethodBase? Target { get; protected set; }

    /// <inheritdoc />
    public HarmonyMethod? Prefix { get; }

    /// <inheritdoc />
    public HarmonyMethod? Postfix { get; }

    /// <inheritdoc />
    public HarmonyMethod? Transpiler { get; }

    /// <inheritdoc />
    public HarmonyMethod? Finalizer { get; }

    /// <inheritdoc />
    public HarmonyMethod? Reverse { get; }

    /// <summary>Gets the <see cref="Harmonizer"/> instance that manages this patcher.</summary>
    protected Harmonizer Harmonizer { get; }

    /// <inheritdoc />
    bool IHarmonyPatcher.Apply(Harmony harmony)
    {
        try
        {
            return this._isApplied = this.ApplyImpl(harmony);
        }
        catch (Exception ex)
        {
            this.Harmonizer.Log.E(ex.ToString());
            return false;
        }
    }

    /// <inheritdoc />
    bool IHarmonyPatcher.Unapply(Harmony harmony)
    {
        try
        {
            return this._isApplied && !(this._isApplied = !this.UnapplyImpl(harmony));
        }
        catch (Exception ex)
        {
            this.Harmonizer.Log.E(ex.ToString());
            return false;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return this.GetType().Name;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        return this.GetType().GetHashCode();
    }

    /// <inheritdoc cref="IHarmonyPatcher.Apply"/>
    protected virtual bool ApplyImpl(Harmony harmony)
    {
        if (this.Target is null)
        {
            return false;
        }

        if (this.Reverse is not null)
        {
            harmony.CreateReversePatcher(this.Target, this.Reverse).Patch();
        }

        harmony.Patch(this.Target, this.Prefix, this.Postfix, this.Transpiler, this.Finalizer);
        return true;
    }

    /// <inheritdoc cref="IHarmonyPatcher.Unapply"/>
    protected virtual bool UnapplyImpl(Harmony harmony)
    {
        if (this.Target is null)
        {
            return false;
        }

        if (this.Prefix is not null)
        {
            harmony.Unpatch(this.Target, this.Prefix.method);
        }

        if (this.Postfix is not null)
        {
            harmony.Unpatch(this.Target, this.Postfix.method);
        }

        if (this.Transpiler is not null)
        {
            harmony.Unpatch(this.Target, this.Transpiler.method);
        }

        return true;
    }

    /// <summary>Gets a constructor and asserts that it was found.</summary>
    /// <typeparam name="TType">The type to search in.</typeparam>
    /// <param name="parameters">The constructor parameter types, or <see langword="null"/> if it's not overloaded.</param>
    /// <returns>The corresponding <see cref="ConstructorInfo"/>.</returns>
    /// <remarks>Originally by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
    protected ConstructorInfo RequireConstructor<TType>(params Type[] parameters)
    {
        return typeof(TType).RequireConstructor(parameters);
    }

    /// <summary>Gets a method and asserts that it was found.</summary>
    /// <typeparam name="TType">The type to search in.</typeparam>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <see langword="null"/> if it's not overloaded.</param>
    /// <returns>The corresponding <see cref="MethodInfo"/>.</returns>
    /// <remarks>Originally by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
    protected MethodInfo RequireMethod<TType>(string name, Type[]? parameters = null)
    {
        return typeof(TType).RequireMethod(name, parameters);
    }

    /// <summary>Gets a property getter and asserts that it was found.</summary>
    /// <typeparam name="TType">The type to search in.</typeparam>
    /// <param name="name">The property name.</param>
    /// <returns>The corresponding <see cref="MethodInfo"/>.</returns>
    /// <remarks>Originally by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
    protected MethodInfo RequirePropertyGetter<TType>(string name)
    {
        return typeof(TType).RequirePropertyGetter(name);
    }

    /// <summary>Gets a property setter and asserts that it was found.</summary>
    /// <typeparam name="TType">The type to search in.</typeparam>
    /// <param name="name">The property name.</param>
    /// <returns>The corresponding <see cref="MethodInfo"/>.</returns>
    /// <remarks>Originally by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
    protected MethodInfo RequirePropertySetter<TType>(string name)
    {
        return typeof(TType).RequirePropertySetter(name);
    }

    /// <summary>Gets all <see cref="HarmonyPatcher"/>-annotated methods in the current instance.</summary>
    /// <returns>The <see cref="HarmonyMethod"/> representations of each patch method within the <see cref="HarmonyPatcher"/>.</returns>
    private (HarmonyMethod? Prefix, HarmonyMethod? Postfix, HarmonyMethod? Transpiler, HarmonyMethod? Finalizer, HarmonyMethod? ReversePatch) GetHarmonyMethods()
    {
        // get all static and private inner methods of this class
        var methods = this.GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic);

        // identify patch methods by custom Harmony annotations and create Harmony Method instances
        var prefix = methods.FirstOrDefault(m => m.GetCustomAttributes(typeof(HarmonyPrefix), false).Length > 0)
            .ToHarmonyMethod();

        var postfix = methods.FirstOrDefault(m => m.GetCustomAttributes(typeof(HarmonyPostfix), false).Length > 0)
            .ToHarmonyMethod();

        var transpiler = methods
            .FirstOrDefault(m => m.GetCustomAttributes(typeof(HarmonyTranspiler), false).Length > 0)
            .ToHarmonyMethod();

        var finalizer = methods
            .FirstOrDefault(m => m.GetCustomAttributes(typeof(HarmonyFinalizer), false).Length > 0)
            .ToHarmonyMethod();

        foreach (var harmony in prefix.Collect(postfix, transpiler, finalizer))
        {
            if (harmony?.method.GetCustomAttribute<HarmonyBefore>() is { } beforeAttribute)
            {
                harmony.before = beforeAttribute.info.before;
            }

            if (harmony?.method.GetCustomAttribute<HarmonyAfter>() is { } afterAttribute)
            {
                harmony.after = afterAttribute.info.after;
            }

            if (harmony?.method.GetCustomAttribute<HarmonyPriority>() is { } priorityAttribute)
            {
                harmony.priority = priorityAttribute.info.priority;
            }
        }

        var reverse = methods
            .FirstOrDefault(m => m.GetCustomAttributes(typeof(HarmonyReversePatch), false).Length > 0)
            .ToHarmonyMethod();

        return (prefix, postfix, transpiler, finalizer, reverse);
    }
}
