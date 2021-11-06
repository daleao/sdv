using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class SkillsPagePerformHoverActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SkillsPagePerformHoverActionPatch()
		{
			Original = RequireMethod<SkillsPage>(nameof(SkillsPage.performHoverAction));
			Postfix = new(AccessTools.Method(GetType(), nameof(SkillsPagePerformHoverActionPostfix)));
		}

		#region harmony patches

		/// <summary>Patch to truncate profession descriptions in hover menu.</summary>
		[HarmonyPostfix]
		private static void SkillsPagePerformHoverActionPostfix(SkillsPage __instance, int x, int y,
			ref string ___hoverText)
		{
			var bounds =
				new Rectangle(
					__instance.xPositionOnScreen + __instance.width - 89,
					__instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth -
					68, 30, 30);

			for (var i = 0; i < 5; ++i)
			{
				bounds.Y += 56;
				if (!bounds.Contains(x, y)) continue;

				var skillIndex = i switch
				{
					1 => 3,
					3 => 1,
					_ => i
				};
				var professionsForThisSkill = Game1.player.GetProfessionsForSkill(skillIndex, true).ToList();
				var count = professionsForThisSkill.Count;
				if (count == 0) continue;

				___hoverText = ModEntry.ModHelper.Translation.Get("prestige.skillpage.tooltip", new {count});
				___hoverText = professionsForThisSkill.Select(Utility.Professions.NameOf)
					.Aggregate(___hoverText, (current, name) => current + $"\n• {name}");
			}

			___hoverText = ___hoverText?.Truncate(90);
		}

		#endregion harmony patches
	}
}