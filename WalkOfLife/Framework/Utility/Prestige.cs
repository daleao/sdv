using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Enums;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Utility
{
	/// <summary>Holds common methods and properties related to prestige elements.</summary>
	public static class Prestige
	{
		/// <summary>The prestige ribbon tilesheet.</summary>
		public static Texture2D RibbonTx { get; } =
			ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "sprites", "ribbons.png"));

		/// <summary>Get the cost of prestiging the specified skill.</summary>
		/// <param name="whichSkill">The desired skill.</param>
		public static int GetPrestigeCost(SkillType whichSkill)
		{
			var count = Game1.player.GetProfessionsForSkill((int) whichSkill, true).Count();
#pragma warning disable 8509
			return count switch
#pragma warning restore 8509
			{
				1 => 10000,
				2 => 50000,
				3 => 100000
			};
		}
	}
}