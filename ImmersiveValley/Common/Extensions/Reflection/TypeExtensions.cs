namespace DaLion.Common.Extensions.Reflection;

#region using directives

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion using directives

/// <summary>Provides extensions for simplified reflection on C# types.</summary>
/// <remarks>Original code by <see href="https://github.com/pardeike">Pardeike</see>.</remarks>
public static class TypeExtensions
{
    /// <summary>Determines whether the current type can be assigned to a variable of any of the candidate types.</summary>
    /// <param name="candidates">The candidate types.</param>
    public static bool IsAssignableToAnyOf(this Type type, params Type[] candidates) =>
        candidates.Any(type.IsAssignableTo);

    /// <summary>Determines whether an instance of any of the candidate types can be assigned to the current type.</summary>
    /// <param name="candidates">The candidate types.</param>
    public static bool IsAssignableFromAnyOf(this Type type, params Type[] candidates) =>
        candidates.Any(type.IsAssignableFrom);

    #region safe reflection

    /// <summary>Get a constructor and assert that it was found.</summary>
    public static ConstructorInfo RequireConstructor(this Type type) =>
        AccessTools.Constructor(type) ??
        throw new MissingMethodException($"Couldn't find constructor for type {type.FullName}.");

    /// <summary>Get a constructor and assert that it was found.</summary>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    public static ConstructorInfo RequireConstructor(this Type type, params Type[]? parameters) =>
        AccessTools.Constructor(type, parameters) ??
        throw new MissingMethodException(
            $"Couldn't find constructor {parameters.Description()} for type {type.FullName}.");

    /// <summary>Get a constructor and assert that it was found.</summary>
    /// <param name="parameterCount">The the number of parameters in the overload signature.</param>
    /// <returns>The first constructor that matches the specified parameter count.</returns>
    /// <remarks>Useful when there's no compile-time access to one or more parameter types.</remarks>
    public static ConstructorInfo RequireConstructor(this Type type, int parameterCount) =>
        AccessTools.GetDeclaredConstructors(type).First(c => c.GetParameters().Length == parameterCount);

    /// <summary>Get a method and assert that it was found.</summary>
    /// <param name="name">The method name.</param>
    public static MethodInfo RequireMethod(this Type type, string name) =>
        AccessTools.Method(type, name) ??
        throw new MissingMethodException($"Couldn't find method {name} in type {type.FullName}.");

    /// <summary>Get a method and assert that it was found.</summary>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    public static MethodInfo RequireMethod(this Type type, string name, Type[]? parameters) =>
        AccessTools.Method(type, name, parameters) ??
        throw new MissingMethodException(
            $"Couldn't find method {name} {parameters.Description()} in type {type.FullName}.");

    /// <summary>Get a field and assert that it was found.</summary>
    /// <param name="name">The field name.</param>
    public static FieldInfo RequireField(this Type type, string name) =>
        AccessTools.Field(type, name) ??
        throw new MissingFieldException($"Couldnd't find field {name} in type {type.FullName}.");

    /// <summary>Get a property getter and assert that it was found.</summary>
    /// <param name="name">The property name.</param>
    public static MethodInfo RequirePropertyGetter(this Type type, string name) =>
        AccessTools.Property(type, name)?.GetGetMethod(true) ??
        throw new MissingMethodException($"Couldn't find property getter {name} in type {type.FullName}.");

    /// <summary>Get a property setter and assert that it was found.</summary>
    /// <param name="name">The property name.</param>
    public static MethodInfo RequirePropertySetter(this Type type, string name) =>
        AccessTools.Property(type, name)?.GetSetMethod(true) ??
        throw new MissingMethodException($"Couldn't find property setter {name} in type {type.FullName}.");

    /// <summary>Get all inner types of a given type.</summary>
    /// <param name="parent">The parent type.</param>
    public static IEnumerable<Type> GetAllInnerTypes(this Type parent)
    {
        yield return parent;
        foreach (var t1 in parent.GetNestedTypes(AccessTools.all))
            foreach (var t2 in GetAllInnerTypes(t1))
                yield return t2;
    }

    /// <summary>Get all inner types starting with a given string.</summary>
    /// <param name="prefix">A string prefix.</param>
    public static List<MethodInfo> GetInnerMethodsStartingWith(this Type type, string prefix)
    {
        var methods = type.GetAllInnerTypes()
            .SelectMany(AccessTools.GetDeclaredMethods)
            .Where(m => prefix == "*" || m.Name.StartsWith(prefix))
            .ToList();
        if (methods.Count <= 0)
            throw new MissingMethodException(
                $"Couldn't find method starting with {prefix} in any inner type of {type.FullName}.");
        return methods;
    }

    #endregion safe reflection

    #region delegate compilation

    /// <summary>Get <see cref="MethodInfo"/> for this delegate type.</summary>
    public static MethodInfo GetMethodInfoFromDelegateType(this Type delegateType)
    {
        //Contract.Requires<ArgumentException>(delegateType.IsSubclassOf(typeof(MulticastDelegate)),
        //    $"{delegateType.Name} is not a delegate type.");
        Debug.Assert(delegateType.IsSubclassOf(typeof(MulticastDelegate)));
        return delegateType.GetMethod("Invoke")!;
    }

    #endregion delegate compilation
}