using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Projectiles;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class BasicProjectileExplodeOnImpact : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BasicProjectileExplodeOnImpact()
		{
			Original = typeof(BasicProjectile).MethodNamed(nameof(BasicProjectile.explodeOnImpact));
			Prefix = new HarmonyMethod(GetType(), nameof(BasicProjectileExplodeOnImpactPrefix));
		}

		#region harmony patches

		/// <summary>Patch to increase Demolitionist explosive ammo radius.</summary>
		[HarmonyPrefix]
		private static bool BasicProjectileExplodeOnImpactPrefix(GameLocation location, int x, int y, Character who)
		{
			try
			{
				if (who is Farmer farmer && farmer.HasProfession("Demolitionist"))
				{
					location.explode(new Vector2(x / Game1.tileSize, y / Game1.tileSize), 3, farmer);
					return false; // don't run original logic
				}

				return true; // run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}