using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class LevelUpMenuDrawPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuDrawPatch()
		{
			Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.draw), new[] {typeof(SpriteBatch)});
			Prefix = new(GetType().MethodNamed(nameof(LevelUpMenuDrawPrefix)));
			Transpiler = new(GetType().MethodNamed(nameof(LevelUpMenuDrawTranspiler)));
		}

		#region harmony patches

		/// <summary>Patch to increase the height of Level Up Menu to fit longer profession descriptions.</summary>
		[HarmonyPrefix]
		private static bool LevelUpMenuDrawPrefix(LevelUpMenu __instance, int ___currentSkill, int ___currentLevel)
		{
			if (__instance.isProfessionChooser && ___currentSkill == 4 && ___currentLevel == 10)
				__instance.height += 32;

			return true; // run original logic
		}

		/// <summary>Patch to draw Prestige tooltip during profession selection.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> LevelUpMenuDrawTranspiler(IEnumerable<CodeInstruction> instructions,
			MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Injected: DrawSubroutine(this, b);
			/// Before: else (!isProfessionChooser)

			try
			{
				helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldfld,
							typeof(LevelUpMenu).Field(nameof(LevelUpMenu.isProfessionChooser)))
					)
					.Advance()
					.GetOperand(out var isNotProfessionChooser)
					.FindLabel((Label) isNotProfessionChooser)
					.Retreat()
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Ldarg_1),
						new CodeInstruction(OpCodes.Call,
							typeof(LevelUpMenuDrawPatch).MethodNamed(nameof(DrawSubroutine)))
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while patching level up menu prestige ribbon draw. Helper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches

		#region private methods

		private static void DrawSubroutine(LevelUpMenu menu, SpriteBatch b)
		{
			if (!menu.isProfessionChooser) return;

			var professionsToChoose = ModEntry.ModHelper.Reflection.GetField<List<int>>(menu, "professionsToChoose")
				.GetValue();
			var leftProfession = professionsToChoose[0];
			var rightProfession = professionsToChoose[1];

			if (Game1.player.professions.Contains(leftProfession) &&
			    Game1.player.HasAllProfessionsInBranch(leftProfession))
			{
				var selectionArea = new Rectangle(menu.xPositionOnScreen + 32, menu.yPositionOnScreen + 232,
					menu.width / 2 - 40, menu.height - 264);
				if (selectionArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
				{
					var hoverText = ModEntry.ModHelper.Translation.Get(leftProfession % 6 <= 1
						? "prestige.levelup.tooltip:5"
						: "prestige.levelup.tooltip:10");
					IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
				}
			}

			if (Game1.player.professions.Contains(rightProfession) &&
			    Game1.player.HasAllProfessionsInBranch(rightProfession))
			{
				var selectionArea = new Rectangle(menu.xPositionOnScreen + menu.width / 2 + 8,
					menu.yPositionOnScreen + 232,
					menu.width / 2 - 40, menu.height - 264);
				if (selectionArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
				{
					var hoverText = ModEntry.ModHelper.Translation.Get(leftProfession % 6 <= 1
						? "prestige.levelup.tooltip:5"
						: "prestige.levelup.tooltip:10");
					IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
				}
			}
		}

		#endregion private methods
	}
}