using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	/// <summary>Base implementation for Harmony patch classes.</summary>
	internal abstract class BasePatch : IPatch
	{
		protected MethodBase Original { get; set; }
		protected HarmonyMethod Prefix { get; set; }
		protected HarmonyMethod Postfix { get; set; }
		protected HarmonyMethod Transpiler { get; set; }

		/// <inheritdoc />
		public virtual void Apply(Harmony harmony)
		{
			if (Original is null)
			{
				ModEntry.Log($"[Patch]: Ignoring {GetType().Name}. The patch target was not found.", LogLevel.Trace);
				return;
			}

			try
			{
				ModEntry.Log($"[Patch]: Applying {GetType().Name} to {Original.DeclaringType}::{Original.Name}.",
					LogLevel.Trace);
				harmony.Patch(Original, Prefix, Postfix, Transpiler);
			}
			catch (Exception ex)
			{
				ModEntry.Log(
					$"[Patch]: Failed to patch {Original.DeclaringType}::{Original.Name}.\nHarmony returned {ex}",
					LogLevel.Error);
			}
		}

		/// <summary>Get a method and assert that it was found.</summary>
		/// <typeparam name="TTarget">The type containing the method.</typeparam>
		/// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
		/// <remarks>Credit to Pathoschild.</remarks>
		protected ConstructorInfo RequireConstructor<TTarget>(params Type[] parameters)
		{
			return typeof(TTarget).Constructor(parameters);
		}

		/// <summary>Get a method and assert that it was found.</summary>
		/// <typeparam name="TTarget">The type containing the method.</typeparam>
		/// <param name="name">The method name.</param>
		/// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
		/// <remarks>Credit to Pathoschild.</remarks>
		protected MethodInfo RequireMethod<TTarget>(string name, Type[] parameters = null)
		{
			return typeof(TTarget).MethodNamed(name, parameters);
		}
	}
}