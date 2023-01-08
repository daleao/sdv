using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace DaLion.Meads;

/// <summary>Provides extensions for simplified reflection on C# types.</summary>
/// <remarks>Credit to <c>Pardeike</c>.</remarks>
public static class TypeExtensions
{
    /// <summary>Get a type by name and assert that it was found.</summary>
    /// <param name="name">The name of the type, preferably with namespace.</param>
    public static Type ToType(this string name)
    {
        return AccessTools.TypeByName(name) ?? throw new($"Cannot find type named {name}.");
    }

    /// <summary>Get a constructor and assert that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    public static ConstructorInfo RequireConstructor(this Type type)
    {
        return AccessTools.Constructor(type) ??
               throw new MissingMethodException($"Cannot find constructor for type {type.FullName}.");
    }

    /// <summary>Get a constructor and assert that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    public static ConstructorInfo RequireConstructor(this Type type, Type[] parameters)
    {
        return AccessTools.Constructor(type, parameters) ??
               throw new MissingMethodException(
                   $"Cannot find constructor {parameters.Description()} for type {type.FullName}.");
    }

    /// <summary>Get a constructor and assert that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="paramCount">The the number of parameters in the overload signature.</param>
    /// <returns>The first constructor that matches the specified parameter count.</returns>
    /// <remarks>Useful when there's no compile-time access to one or more parameter types.</remarks>
    public static ConstructorInfo RequireConstructor(this Type type, int paramCount)
    {
        return AccessTools.GetDeclaredConstructors(type).First(c => c.GetParameters().Length == paramCount);
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The method name.</param>
    public static MethodInfo RequireMethod(this Type type, string name)
    {
        return AccessTools.Method(type, name) ??
               throw new MissingMethodException($"Cannot find method named {name} in type {type.FullName}.");
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    public static MethodInfo RequireMethod(this Type type, string name, Type[] parameters)
    {
        return AccessTools.Method(type, name, parameters) ??
               throw new MissingMethodException(
                   $"Cannot find method {name} {parameters.Description()} in type {type.FullName}.");
    }

    /// <summary>Get a field and assert that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The field name.</param>
    public static FieldInfo RequireField(this Type type, string name)
    {
        return AccessTools.Field(type, name) ??
               throw new MissingFieldException($"Cannot find field {name} in type {type.FullName}.");
    }

    /// <summary>Get a property getter and assert that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The property name.</param>
    public static MethodInfo RequirePropertyGetter(this Type type, string name)
    {
        return AccessTools.Property(type, name)?.GetGetMethod(true) ??
               throw new MissingMethodException($"Cannot find property getter {name} in type {type.FullName}.");
    }

    /// <summary>Get a property setter and assert that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The property name.</param>
    public static MethodInfo RequirePropertySetter(this Type type, string name)
    {
        return AccessTools.Property(type, name)?.GetSetMethod(true) ??
               throw new MissingMethodException($"Cannot find property setter {name} in type {type.FullName}.");
    }
}