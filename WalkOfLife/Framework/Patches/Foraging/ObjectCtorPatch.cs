using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ObjectCtorPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Constructor(typeof(SObject), new[] { typeof(Vector2), typeof(int), typeof(string), typeof(bool), typeof(bool), typeof(bool), typeof(bool) }),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectCtorPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Ecologist wild berry recovery.</summary>
		private static void ObjectCtorPostfix(ref SObject __instance)
		{
			try
			{
				var owner = Game1.getFarmer(__instance.owner.Value);
				if (Utility.IsWildBerry(__instance) && Utility.SpecificPlayerHasProfession("Ecologist", owner))
					__instance.Edibility = (int)(__instance.Edibility * 1.5f);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(ObjectCtorPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}