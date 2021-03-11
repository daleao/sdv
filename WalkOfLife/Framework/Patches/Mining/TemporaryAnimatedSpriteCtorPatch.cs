using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class TemporaryAnimatedSpriteCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal TemporaryAnimatedSpriteCtorPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(TemporaryAnimatedSprite), new Type[] { typeof(int), typeof(float), typeof(int), typeof(int), typeof(Vector2), typeof(bool), typeof(bool), typeof(GameLocation), typeof(Farmer) }),
				postfix: new HarmonyMethod(GetType(), nameof(TemporaryAnimatedSpriteCtorPostfix))
			);
		}

		/// <summary>Patch to increase Demolitionist bomb radius.</summary>
		protected static void TemporaryAnimatedSpriteCtorPostfix(ref TemporaryAnimatedSprite __instance, Farmer owner)
		{
			if (Utils.SpecificPlayerHasProfession("demolitionist", owner))
			{
				++__instance.bombRadius;
			}
		}
	}
}
