using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace TheLion.Stardew.Common.Harmony
{
	/// <remarks>Credit to <c>Pardeike</c>.</remarks>
	public static class SafeReflections
	{
		/// <summary>Get a type by name and assert that it was found.</summary>
		public static Type ToType(this string name)
		{
			return AccessTools.TypeByName(name) ?? throw new($"Cannot find type named {name}.");
		}

		/// <summary>Get a constructor and assert that it was found.</summary>
		public static ConstructorInfo Constructor(this Type type)
		{
			return AccessTools.Constructor(type) ?? throw new($"Cannot find constructor for type {type.FullName}.");
		}

		/// <summary>Get a constructor and assert that it was found.</summary>
		/// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
		public static ConstructorInfo Constructor(this Type type, Type[] parameters)
		{
			return AccessTools.Constructor(type, parameters) ??
			       throw new($"Cannot find constructor {parameters.Description()} for type {type.FullName}.");
		}

		/// <summary>Get a method and assert that it was found.</summary>
		/// <param name="name">The method name.</param>
		public static MethodInfo MethodNamed(this Type type, string name)
		{
			return AccessTools.Method(type, name) ??
			       throw new($"Cannot find method named {name} in type {type.FullName}.");
		}

		/// <summary>Get a method and assert that it was found.</summary>
		/// <param name="name">The method name.</param>
		/// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
		public static MethodInfo MethodNamed(this Type type, string name, Type[] parameters)
		{
			return AccessTools.Method(type, name, parameters) ??
			       throw new($"Cannot find method {name} {parameters.Description()} in type {type.FullName}.");
		}

		/// <summary>Get a field and assert that it was found.</summary>
		/// <param name="name">The field name.</param>
		public static FieldInfo Field(this Type type, string name)
		{
			return AccessTools.Field(type, name) ??
			       throw new($"Cannot find field {name} in type {type.FullName}.");
		}

		/// <summary>Get a property getter and assert that it was found.</summary>
		/// <param name="name">The property name.</param>
		public static MethodInfo PropertyGetter(this Type type, string name)
		{
			return AccessTools.Property(type, name)?.GetGetMethod(true) ??
			       throw new($"Cannot find property getter {name} in type {type.FullName}.");
		}

		/// <summary>Get a property setter and assert that it was found.</summary>
		/// <param name="name">The property name.</param>
		public static MethodInfo PropertySetter(this Type type, string name)
		{
			return AccessTools.Property(type, name)?.GetSetMethod(true) ??
			       throw new($"Cannot find property getter {name} in type {type.FullName}.");
		}

		/// <summary>Get all inner types of a given type.</summary>
		/// <param name="parent">The parent type.</param>
		public static IEnumerable<Type> GetAllInnerTypes(Type parent)
		{
			yield return parent;
			foreach (var t1 in parent.GetNestedTypes(AccessTools.all))
			foreach (var t2 in GetAllInnerTypes(t1))
				yield return t2;
		}

		/// <summary>Get all inner types starting with a given string.</summary>
		/// <param name="prefix">A string prefix.</param>
		public static List<MethodInfo> InnerMethodsStartingWith(this Type type, string prefix)
		{
			var methods = GetAllInnerTypes(type)
				.SelectMany(AccessTools.GetDeclaredMethods)
				.Where(m => prefix == "*" || m.Name.StartsWith(prefix))
				.ToList();
			if (!methods.Any())
				throw new($"Cannot find method starting with {prefix} in any inner type of {type.FullName}.");
			return methods;
		}

		/// <summary>Construct a <see cref="HarmonyMethod" /> instance from a <see cref="MethodInfo" /> object.</summary>
		/// <returns>
		///     Returns a new <see cref="HarmonyMethod" /> instance if <paramref name="method" /> is not null, or <c>null</c>
		///     otherwise.
		/// </returns>
		[CanBeNull]
		public static HarmonyMethod ToHarmonyMethod(this MethodInfo method)
		{
			return method is null ? null : new HarmonyMethod(method);
		}
	}
}