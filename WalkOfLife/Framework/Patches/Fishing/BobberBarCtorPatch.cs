using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class BobberBarCtorPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal BobberBarCtorPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(BobberBar), new Type[] { typeof(int), typeof(float), typeof(bool), typeof(int) }),
				transpiler: new HarmonyMethod(GetType(), nameof(BobberBarCtorTranspiler))
			);
		}

		/// <summary>Patch for Aquarist bobber bar height.</summary>
		protected static IEnumerable<CodeInstruction> BobberBarCtorTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(BobberBar)}::{nameof(BobberBar)}.");

			/// Injected: bobberBarHeight += _GetBonusBobberHeight()
			try
			{
				_helper
					.Find(
						new CodeInstruction(OpCodes.Ldc_I4, operand: 695)	// find index of cork bobber check
					)
					.Advance(2)
					.ToBufferUntil(											// copy bobberBarHeight increment
						new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(BobberBar), name: "bobberBarHeight"))
					)
					.InsertBuffer()											// paste
					.AdvanceUntil(											// cork bobber bonus
						new CodeInstruction(OpCodes.Ldc_I4_S, operand: 24)
					)
					.ReplaceWith(											// replace with Aquarist bonus
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BobberBarCtorPatch), nameof(BobberBarCtorPatch._GetBonusBobberBarHeight)))
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while adding Aquarist bonus bobber bar height.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
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
