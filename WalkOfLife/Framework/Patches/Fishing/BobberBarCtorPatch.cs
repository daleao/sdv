using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class BobberBarCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal BobberBarCtorPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(BobberBar), new Type[] { typeof(int), typeof(float), typeof(bool), typeof(int) }),
				postfix: new HarmonyMethod(GetType(), nameof(BobberBarCtorPostfix))
			);
		}

		/// <summary>Patch for Aquarist bonus bobber height.</summary>
		protected static void BobberBarCtorPostfix(ref BobberBar __instance, ref int ___bobberBarHeight, ref float ___bobberBarPos)
		{
			int bonusBobberHeight = _GetBonusBobberBarHeight();
			___bobberBarHeight += bonusBobberHeight;
			___bobberBarPos -= bonusBobberHeight;
		}

		/// <summary>Get the bonus bobber bar height for Aquarist.</summary>
		private static int _GetBonusBobberBarHeight()
		{
			if (!Utils.LocalPlayerHasProfession("aquarist"))
			{
				return 0;
			}

			int bonusBobberHeight = 0;
			foreach(Building b in Game1.getFarm().buildings)
			{
				if ((b.owner.Equals(Game1.player.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b is FishPond && (b as FishPond).FishCount == 10)
				{
					bonusBobberHeight += 7;
				}
			}

			return bonusBobberHeight;
		}
	}
}
