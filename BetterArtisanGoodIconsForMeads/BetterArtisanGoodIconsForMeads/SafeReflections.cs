using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace BetterArtisanGoodIconsForMeads;

/// <summary>Provides extensions for simplified reflection on C# types.</summary>
/// <remarks>Credit to <c>Pardeike</c>.</remarks>
public static class TypeExtensions
{
    /// <summary>Get a type by name and assert that it was found.</summary>
    public static Type ToType(this string name)
    {
        return AccessTools.TypeByName(name) ?? throw new($"Cannot find type named {name}.");
    }

    /// <summary>Get a constructor and assert that it was found.</summary>
    public static ConstructorInfo RequireConstructor(this Type type)
    {
        return AccessTools.Constructor(type) ??
               throw new MissingMethodException($"Cannot find constructor for type {type.FullName}.");
    }

    /// <summary>Get a constructor and assert that it was found.</summary>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    public static ConstructorInfo RequireConstructor(this Type type, Type[] parameters)
    {
        return AccessTools.Constructor(type, parameters) ??
               throw new MissingMethodException(
                   $"Cannot find constructor {parameters.Description()} for type {type.FullName}.");
    }

    /// <summary>Get a constructor and assert that it was found.</summary>
    /// <param name="parametersCount">The the number of parameters in the overload signature.</param>
    public static ConstructorInfo RequireConstructor(this Type type, int parametersCount)
    {
        return AccessTools.GetDeclaredConstructors(type).First(c => c.GetParameters().Length == parametersCount);
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <param name="name">The method name.</param>
    public static MethodInfo RequireMethod(this Type type, string name)
    {
        return AccessTools.Method(type, name) ??
               throw new MissingMethodException($"Cannot find method named {name} in type {type.FullName}.");
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    public static MethodInfo RequireMethod(this Type type, string name, Type[] parameters)
    {
        return AccessTools.Method(type, name, parameters) ??
               throw new MissingMethodException(
                   $"Cannot find method {name} {parameters.Description()} in type {type.FullName}.");
    }

    /// <summary>Get a field and assert that it was found.</summary>
    /// <param name="name">The field name.</param>
    public static FieldInfo RequireField(this Type type, string name)
    {
        return AccessTools.Field(type, name) ??
               throw new MissingFieldException($"Cannot find field {name} in type {type.FullName}.");
    }

    /// <summary>Get a property getter and assert that it was found.</summary>
    /// <param name="name">The property name.</param>
    public static MethodInfo RequirePropertyGetter(this Type type, string name)
    {
        return AccessTools.Property(type, name)?.GetGetMethod(true) ??
               throw new MissingMethodException($"Cannot find property getter {name} in type {type.FullName}.");
    }

    /// <summary>Get a property setter and assert that it was found.</summary>
    /// <param name="name">The property name.</param>
    public static MethodInfo RequirePropertySetter(this Type type, string name)
    {
        return AccessTools.Property(type, name)?.GetSetMethod(true) ??
               throw new MissingMethodException($"Cannot find property setter {name} in type {type.FullName}.");
    }

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
        if (!methods.Any())
            throw new MissingMethodException(
                $"Cannot find method starting with {prefix} in any inner type of {type.FullName}.");
        return methods;
    }
}