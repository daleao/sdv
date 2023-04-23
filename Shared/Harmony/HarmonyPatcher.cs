namespace DaLion.Shared.Harmony;

#region using directives

using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Base implementation of a <see cref="Harmony"/> patch class targeting a single method.</summary>
internal abstract class HarmonyPatcher : IHarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="HarmonyPatcher"/> class.</summary>
    protected HarmonyPatcher()
    {
        (this.Prefix, this.Postfix, this.Transpiler, this.Finalizer, this.Reverse) = this.GetHarmonyMethods();
    }

    /// <inheritdoc />
    public MethodBase Target { get; protected set; } = null!; // initialized in derived class

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

    /// <summary>Gets or sets the method that is currently being patched.</summary>
    protected internal static MethodBase? NowPatching { get; protected set; }

    /// <inheritdoc />
    bool IHarmonyPatcher.Apply(Harmony harmony)
    {
        try
        {
            return this.ApplyImpl(harmony);
        }
        catch (Exception ex)
        {
            Log.E(ex.ToString());
            return false;
        }
    }

    /// <inheritdoc />
    bool IHarmonyPatcher.Unapply(Harmony harmony)
    {
        try
        {
            this.UnapplyImpl(harmony);
            return true;
        }
        catch (Exception ex)
        {
            Log.E(ex.ToString());
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
        NowPatching = this.Target;

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
    /// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
    protected ConstructorInfo RequireConstructor<TType>(params Type[] parameters)
    {
        return typeof(TType).RequireConstructor(parameters);
    }

    /// <summary>Gets a method and asserts that it was found.</summary>
    /// <typeparam name="TType">The type to search in.</typeparam>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <returns>The corresponding <see cref="MethodInfo"/>.</returns>
    /// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
    protected MethodInfo RequireMethod<TType>(string name, Type[]? parameters = null)
    {
        return typeof(TType).RequireMethod(name, parameters);
    }

    /// <summary>Gets a property getter and asserts that it was found.</summary>
    /// <typeparam name="TType">The type to search in.</typeparam>
    /// <param name="name">The property name.</param>
    /// <returns>The corresponding <see cref="MethodInfo"/>.</returns>
    /// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
    protected MethodInfo RequirePropertyGetter<TType>(string name)
    {
        return typeof(TType).RequirePropertyGetter(name);
    }

    /// <summary>Gets a property setter and asserts that it was found.</summary>
    /// <typeparam name="TType">The type to search in.</typeparam>
    /// <param name="name">The property name.</param>
    /// <returns>The corresponding <see cref="MethodInfo"/>.</returns>
    /// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
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
        var reverse = methods
            .FirstOrDefault(m => m.GetCustomAttributes(typeof(HarmonyReversePatch), false).Length > 0)
            .ToHarmonyMethod();

        return (prefix, postfix, transpiler, finalizer, reverse);
    }
}
