namespace DaLion.Common.Harmony;

#region using directives

using System;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Reflection;
using HarmonyLib;

using Extensions.Reflection;

#endregion using directives

/// <summary>Base implementation for Harmony patches.</summary>
internal abstract class BasePatch : IPatch
{
    /// <inheritdoc />
    public MethodBase Target { get; protected init; }

    /// <inheritdoc />
    public HarmonyMethod Prefix { get; }

    /// <inheritdoc />
    public HarmonyMethod Postfix { get; }

    /// <inheritdoc />
    public HarmonyMethod Transpiler { get; }

    /// <inheritdoc />
    public HarmonyMethod Reverse { get; }

    /// <summary>Construct an instance.</summary>
    protected BasePatch()
    {
        (Prefix, Postfix, Transpiler, Reverse) = GetHarmonyMethods();
    }

    /// <inheritdoc />
    void IPatch.Apply(Harmony harmony)
    {
        if (Target is null) throw new MissingMethodException();

        harmony.Patch(Target, Prefix, Postfix, Transpiler);
        if (Reverse is not null)
            harmony.CreateReversePatcher(Target, Reverse);
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <remarks><see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
    protected ConstructorInfo RequireConstructor<TType>(params Type[] parameters)
    {
        return typeof(TType).RequireConstructor(parameters);
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <remarks><see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
    protected MethodInfo RequireMethod<TType>(string name, Type[] parameters = null)
    {
        return typeof(TType).RequireMethod(name, parameters);
    }

    /// <summary>Get all Harmony patch methods in the current patch instance.</summary>
    private (HarmonyMethod, HarmonyMethod, HarmonyMethod, HarmonyMethod) GetHarmonyMethods()
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
        var reverse = methods
            .FirstOrDefault(m => m.GetCustomAttributes(typeof(HarmonyReversePatch), false).Length > 0)
            .ToHarmonyMethod();

        return (prefix, postfix, transpiler, reverse);
    }

    /// <inheritdoc />
    public override string ToString() => GetType().Name;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => GetType().GetHashCode();
}