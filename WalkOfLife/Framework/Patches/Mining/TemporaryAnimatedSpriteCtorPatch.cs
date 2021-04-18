using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class TemporaryAnimatedSpriteCtorPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Constructor(typeof(TemporaryAnimatedSprite), new[] { typeof(int), typeof(float), typeof(int), typeof(int), typeof(Vector2), typeof(bool), typeof(bool), typeof(GameLocation), typeof(Farmer) }),
				postfix: new HarmonyMethod(GetType(), nameof(TemporaryAnimatedSpriteCtorPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to increase Demolitionist bomb radius.</summary>
		private static void TemporaryAnimatedSpriteCtorPostfix(ref TemporaryAnimatedSprite __instance, Farmer owner)
		{
			try
			{
				if (Utility.SpecificPlayerHasProfession("Demolitionist", owner)) ++__instance.bombRadius;
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(TemporaryAnimatedSpriteCtorPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}