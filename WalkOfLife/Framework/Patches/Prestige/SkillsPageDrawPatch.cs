using System;
using System.Collections.Generic;
using System.Linq;
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
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class SkillsPageDrawPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SkillsPageDrawPatch()
		{
			Original = RequireMethod<SkillsPage>(nameof(SkillsPage.draw), new[] {typeof(SpriteBatch)});
			Transpiler = new(AccessTools.Method(GetType(), nameof(SkillsPageDrawTranspiler)));
		}

		#region harmony patches

		/// <summary>Patch to draw prestige ribbons on the skills page.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> SkillsPageDrawTranspiler(IEnumerable<CodeInstruction> instructions,
			MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Injected: DrawSubroutine(b);
			/// Before: if (hoverText.Length > 0)

			try
			{
				helper
					.FindLast(
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Ldfld, typeof(SkillsPage).Field("hoverText")),
						new CodeInstruction(OpCodes.Callvirt, typeof(string).PropertyGetter(nameof(string.Length)))
					)
					.StripLabels(out var labels) // backup and remove branch labels
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Ldarg_1),
						new CodeInstruction(OpCodes.Call,
							typeof(SkillsPageDrawPatch).MethodNamed(nameof(DrawSubroutine)))
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while patching skill page prestige ribbon draw. Helper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches

		#region private methods

		private static void DrawSubroutine(SkillsPage page, SpriteBatch b)
		{
			for (var i = 0; i < 5; ++i)
			{
				var position =
					new Vector2(
						page.xPositionOnScreen + page.width - 95,
						page.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth -
						12 + i * 56);

				var skillIndex = i switch
				{
					1 => 3,
					3 => 1,
					_ => i
				};
				var count = Game1.player.GetProfessionsForSkill(skillIndex, true).Count();
				if (count == 0) continue;

				var srcRect = new Rectangle(i * 30, (count - 1) * 30, 30, 30);
				b.Draw(Prestige.RibbonTx, position, srcRect, Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None,
					3f);
			}
		}

		#endregion private methods
	}
}