using System.IO;
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

		public static Texture2D SkillBarTx { get; } =
			ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "menus", "skillbars.png"));

		public static int RibbonWidth => 22;
		public static int RibbonHorizontalOffset => -99;
		public static float RibbonScale => 1.8f;

		/// <summary>Get the cost of prestiging the specified skill.</summary>
		/// <param name="skillType">The desired skill.</param>
		public static int GetPrestigeCost(SkillType skillType)
		{
			var multiplier = ModEntry.Config.PrestigeCostMultiplier;
			if (multiplier <= 0f) return 0;

			var count = Game1.player.NumberOfProfessionsInSkill((int) skillType, true);
#pragma warning disable 8509
			var baseCost = count switch
#pragma warning restore 8509
			{
				1 => 10000,
				2 => 50000,
				3 => 100000
			};

			return (int) (baseCost * multiplier);
		}
	}
}