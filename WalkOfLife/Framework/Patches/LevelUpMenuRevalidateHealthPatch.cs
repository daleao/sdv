using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class LevelUpMenuRevalidateHealthPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal LevelUpMenuRevalidateHealthPatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.RevalidateHealth)),
				transpiler: new HarmonyMethod(GetType(), nameof(LevelUpMenuRevalidateHealthTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(LevelUpMenuRevalidateHealthPostfix))
			);
		}

		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		protected static IEnumerable<CodeInstruction> LevelUpMenuRevalidateHealthTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(LevelUpMenu)}::{nameof(LevelUpMenu.RevalidateHealth)}.");

			/// From: if (farmer.professions.Contains(<defender_id>))
			/// To: if (farmer.professions.Contains(<brute_id>))

			try
			{
				_helper
					.FindProfessionCheck(Farmer.defender)
					.Advance()
					.SetOperand(Utils.ProfessionMap.Forward["brute"]);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Patch revalidate modded immediate profession perks.</summary>
		protected static void LevelUpMenuRevalidateHealthPostfix(Farmer farmer)
		{
			// revalidate Angler tackle health
			int expectedMaxTackleUses = 20;
			if (Utils.SpecificPlayerHasProfession("angler", farmer))
				expectedMaxTackleUses *= 2;

			FishingRod.maxTackleUses = expectedMaxTackleUses;

			// revalidate Aquarist max fish pond capacity
			if (Utils.SpecificPlayerHasProfession("aquarist", farmer))
			{
				foreach (Building b in Game1.getFarm().buildings)
				{
					if ((b.owner.Equals(farmer.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b is FishPond && b.maxOccupants.Value < 12)
						(b as FishPond).UpdateMaximumOccupancy();
				}
			}
		}
	}
}
