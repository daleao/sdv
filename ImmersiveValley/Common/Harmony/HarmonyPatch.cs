namespace DaLion.Common.Harmony;

#region using directives

using Extensions.Reflection;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#endregion using directives

/// <summary>Base implementation of a <see cref="Harmony"/> patch class targeting a single method.</summary>
internal abstract class HarmonyPatch : IHarmonyPatch
{
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

    /// <summary>Construct an instance.</summary>
    protected HarmonyPatch()
    {
        (Prefix, Postfix, Transpiler, Finalizer, Reverse) = GetHarmonyMethods();
    }

    /// <inheritdoc />
    void IHarmonyPatch.Apply(Harmony harmony)
    {
        ApplyImpl(harmony);
    }

    /// <inheritdoc cref="IHarmonyPatch.Apply"/>
    protected virtual void ApplyImpl(Harmony harmony)
    {
        if (Target is null) throw new MissingMethodException("Patch target not defined.");

        harmony.Patch(Target, Prefix, Postfix, Transpiler, Finalizer);
        if (Reverse is null) return;

        var patcher = harmony.CreateReversePatcher(Target, Reverse);
        patcher.Patch();
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
    protected ConstructorInfo RequireConstructor<TType>(params Type[] parameters) =>
        typeof(TType).RequireConstructor(parameters);

    /// <summary>Get a method and assert that it was found.</summary>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
    protected MethodInfo RequireMethod<TType>(string name, Type[]? parameters = null) =>
        typeof(TType).RequireMethod(name, parameters);

    /// <summary>Get all Harmony patch methods in the current patch instance.</summary>
    private (HarmonyMethod?, HarmonyMethod?, HarmonyMethod?, HarmonyMethod?, HarmonyMethod?) GetHarmonyMethods()
    {
        // get all static and private inner methods of this class
        var methods = GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic);

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

    /// <inheritdoc />
    public override string ToString() => GetType().Name;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => GetType().GetHashCode();
}