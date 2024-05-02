namespace DaLion.Shared.Extensions.Reflection;

#region using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Provides extensions for simplified reflection on C# types.</summary>
/// <remarks>Original code by <see href="https://github.com/pardeike">Pardeike</see>.</remarks>
public static class TypeExtensions
{
    /// <summary>Determines whether the <paramref name="type"/> can be assigned to any of the <paramref name="candidates"/>.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="candidates">The candidate types.</param>
    /// <returns><see langword="true"/> if the <paramref name="type"/> is assignable to at least one of the <paramref name="candidates"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsAssignableToAnyOf(this Type type, params Type[] candidates)
    {
        return candidates.Any(type.IsAssignableTo);
    }

    /// <summary>Determines whether any of the <paramref name="candidates"/> can be assigned to the <paramref name="type"/>.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="candidates">The candidate types.</param>
    /// <returns><see langword="true"/> if the <paramref name="type"/> is assignable from at least one of the <paramref name="candidates"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsAssignableFromAnyOf(this Type type, params Type[] candidates)
    {
        return candidates.Any(type.IsAssignableFrom);
    }

    /// <summary>Enumerates all the types which derive from <paramref name="type"/>.</summary>
    /// <param name="type">The base <see cref="Type"/>.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> containing all types assignable to <paramref name="type"/>.</returns>
    public static IEnumerable<Type> GetDerivedTypes(this Type type)
    {
        return AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(type))
            .Where(t => t.IsAssignableTo(type));
    }

    /// <summary>Determines whether the <paramref name="type"/> is overriding the base class definition of the specified method.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="methodName">The name of the method to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="type"/>'s definition of <paramref name="methodName"/> exists and is different from its base class definition, otherwise <see langword="false"/>.</returns>
    public static bool OverridesMethod(this Type type, string methodName)
    {
        return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.IsOverride() == true;
    }

    /// <summary>Determines whether the <paramref name="type"/> is decorated with an <see cref="Attribute"/> of the the specified <paramref name="attributeType"/>.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="attributeType">An <see cref="Attribute"/> <see cref="Type"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="type"/> is decorated with <paramref name="attributeType"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasAttribute(this Type type, Type attributeType)
    {
        return type.GetCustomAttribute(attributeType) is not null;
    }

    /// <summary>Determines whether the <paramref name="type"/> is decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> <see cref="Type"/>.</typeparam>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="type"/> is decorated with <typeparamref name="TAttribute"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasAttribute<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        return type.GetCustomAttribute<TAttribute>() is not null;
    }

    /// <summary>Determines whether the <paramref name="type"/> is decorated with any <see cref="Attribute"/> of the the specified <paramref name="attributeTypes"/>.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="attributeTypes">The candidate <see cref="Attribute"/> types to search for.</param>
    /// <returns><see langword="true"/> if the <paramref name="type"/> is decorated with at least one of the specified <paramref name="attributeTypes"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasAnyAttributeOf(this Type type, params Type[] attributeTypes)
    {
        return attributeTypes.Any(type.HasAttribute);
    }

    #region delegate compilation

    /// <summary>Gets the <see cref="MethodInfo"/> for the <paramref name="delegateType"/>.</summary>
    /// <param name="delegateType">The delegate <see cref="Type"/>.</param>
    /// <returns>The corresponding <see cref="MethodInfo"/>.</returns>
    public static MethodInfo GetMethodInfoFromDelegateType(this Type delegateType)
    {
        return delegateType.GetMethod("Invoke")!;
    }

    #endregion delegate compilation

    #region safe reflection

    /// <summary>Gets a constructor and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <returns>The corresponding <see cref="ConstructorInfo"/>, if found.</returns>
    /// <exception cref="MissingMethodException">If a constructor is not found.</exception>
    [DebuggerStepThrough]
    public static ConstructorInfo RequireConstructor(this Type type)
    {
        return AccessTools.Constructor(type) ??
               ThrowHelper.ThrowMissingMethodException<ConstructorInfo>(
                   $"Couldn't find constructor for type {type.FullName}.");
    }

    /// <summary>Gets a constructor with the specified <paramref name="parameters"/> and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="parameters">The method parameter types, or <see langword="null"/> if it's not overloaded.</param>
    /// <returns>The corresponding <see cref="ConstructorInfo"/>, if found.</returns>
    /// <exception cref="MissingMethodException">If a matching constructor is not found.</exception>
    [DebuggerStepThrough]
    public static ConstructorInfo RequireConstructor(this Type type, params Type[]? parameters)
    {
        return AccessTools.Constructor(type, parameters) ??
               ThrowHelper.ThrowMissingMethodException<ConstructorInfo>(
                   $"Couldn't find constructor {parameters.Description()} for type {type.FullName}.");
    }

    /// <summary>Gets a constructor and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="parameterCount">The the number of parameters in the overload signature.</param>
    /// <returns>The first constructor that matches the specified parameter count.</returns>
    /// <remarks>Useful when there's no compile-time access to one or more parameter types.</remarks>
    [DebuggerStepThrough]
    public static ConstructorInfo RequireConstructor(this Type type, int parameterCount)
    {
        return AccessTools.GetDeclaredConstructors(type).First(c => c.GetParameters().Length == parameterCount);
    }

    /// <summary>Gets a method and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <see langword="null"/> if it's not overloaded.</param>
    /// <param name="generics">The method generic types, if any.</param>
    /// <returns>The corresponding <see cref="MethodInfo"/>, if found.</returns>
    /// <exception cref="MissingMethodException">If a matching method is not found.</exception>
    [DebuggerStepThrough]
    public static MethodInfo RequireMethod(this Type type, string name, Type[]? parameters = null, Type[]? generics = null)
    {
        return AccessTools.Method(type, name, parameters, generics) ??
               ThrowHelper.ThrowMissingMethodException<MethodInfo>(
                   $"Couldn't find method {name} {parameters.Description()} in type {type.FullName}.");
    }

    /// <summary>Gets a field and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The field name.</param>
    /// <returns>The corresponding <see cref="FieldInfo"/>, if found.</returns>
    /// <exception cref="MissingFieldException">If a matching field is not found.</exception>
    [DebuggerStepThrough]
    public static FieldInfo RequireField(this Type type, string name)
    {
        return AccessTools.Field(type, name) ??
               ThrowHelper.ThrowMissingFieldException<FieldInfo>(
                   $"Couldn't find field {name} in type {type.FullName}.");
    }

    /// <summary>Gets a property getter and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The property name.</param>
    /// <returns>The <see cref="MethodInfo"/> corresponding to the getter of the property, if found.</returns>
    /// <exception cref="MissingMethodException">If a matching property is not found.</exception>
    [DebuggerStepThrough]
    public static MethodInfo RequirePropertyGetter(this Type type, string name)
    {
        return AccessTools.Property(type, name)?.GetGetMethod(true) ??
               ThrowHelper.ThrowMissingMethodException<MethodInfo>(
                   $"Couldn't find property getter {name} in type {type.FullName}.");
    }

    /// <summary>Gets a property setter and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The property name.</param>
    /// <returns>The <see cref="MethodInfo"/> corresponding to the setter of the property, if found.</returns>
    /// <exception cref="MissingMethodException">If a matching property is not found.</exception>
    [DebuggerStepThrough]
    public static MethodInfo RequirePropertySetter(this Type type, string name)
    {
        return AccessTools.Property(type, name)?.GetSetMethod(true) ??
               ThrowHelper.ThrowMissingMethodException<MethodInfo>(
                   $"Couldn't find property setter {name} in type {type.FullName}.");
    }

    /// <summary>Gets all inner types of <paramref name="parent"/>.</summary>
    /// <param name="parent">The parent <see cref="Type"/>.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all the inner <see cref="Type"/>s of <paramref name="parent"/>.</returns>
    public static IEnumerable<Type> GetAllInnerTypes(this Type parent)
    {
        yield return parent;

        var nested = parent.GetNestedTypes(AccessTools.all);
        foreach (var t in nested)
        {
            foreach (var inner in GetAllInnerTypes(t))
            {
                yield return inner;
            }
        }
    }

    /// <summary>Gets all inner methods whose names contain the given <paramref name="substring"/>.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="substring">A substring to filter by, or "*" to get all methods.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all the inner <see cref="MethodInfo"/>s of <paramref name="type"/> containing the <paramref name="substring"/>.</returns>
    /// <exception cref="MissingMethodException">If a matching method is not found.</exception>
    public static IEnumerable<MethodInfo> GetInnerMethodsContaining(this Type type, string substring = "*")
    {
        return type.GetAllInnerTypes()
            .SelectMany(AccessTools.GetDeclaredMethods)
            .Where(m => substring == "*" || m.Name.Contains(substring));
    }

    #endregion safe reflection
}
