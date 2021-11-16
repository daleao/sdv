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

		/// <summary>Construct an instance.</summary>
		protected BasePatch()
		{
			(Prefix, Postfix, Transpiler) = GetHarmonyMethods();
		}

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

		/// <summary>Get all Harmony patch methods in the current patch instance.</summary>
		protected (HarmonyMethod, HarmonyMethod, HarmonyMethod) GetHarmonyMethods()
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

			return (prefix, postfix, transpiler);
		}

	}
}