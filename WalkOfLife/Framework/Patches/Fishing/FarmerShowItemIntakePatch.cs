using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using System.IO;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	internal class FarmerShowItemIntakePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Farmer), nameof(Farmer.showItemIntake)),
				prefix: new HarmonyMethod(GetType(), nameof(FarmerShowItemIntakePrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch to show weapons during crab pot harvest animation.</summary>
		private static bool FarmerShowItemIntakePrefix(Farmer who)
		{
			if (!who.mostRecentlyGrabbedItem.ParentSheetIndex.AnyOf(14, 51)) return true; // run original logic

			TemporaryAnimatedSprite tempSprite = null;
			Object toShow = (Object)who.mostRecentlyGrabbedItem;
			switch (who.FacingDirection)
			{
				case 2:
					tempSprite = who.FarmerSprite.currentAnimationIndex switch
					{
						1 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(0f, -32f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						2 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(0f, -43f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						3 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(0f, -128f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						4 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 200f, 1, 0, who.Position + new Vector2(0f, -120f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						5 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 200f, 1, 0, who.Position + new Vector2(0f, -120f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0.02f, Color.White, 4f, -0.02f, 0f, 0f),
						_ => null
					};
					break;

				case 1:
					tempSprite = who.FarmerSprite.currentAnimationIndex switch
					{
						1 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(28f, -64f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						2 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(24f, -72f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						3 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(4f, -128f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						4 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 200f, 1, 0, who.Position + new Vector2(0f, -124f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						5 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 200f, 1, 0, who.Position + new Vector2(0f, -124f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0.02f, Color.White, 4f, -0.02f, 0f, 0f),
						_ => null
					};
					break;

				case 0:
					tempSprite = who.FarmerSprite.currentAnimationIndex switch
					{
						1 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(0f, -32f), flicker: false, flipped: false,
							who.getStandingY() / 10000f - 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f),
						2 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(0f, -43f), flicker: false, flipped: false,
							who.getStandingY() / 10000f - 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f),
						3 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(0f, -128f), flicker: false, flipped: false,
							who.getStandingY() / 10000f - 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f),
						4 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 200f, 1, 0, who.Position + new Vector2(0f, -120f), flicker: false, flipped: false,
							who.getStandingY() / 10000f - 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f),
						5 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 200f, 1, 0, who.Position + new Vector2(0f, -120f), flicker: false, flipped: false,
							who.getStandingY() / 10000f - 0.001f, 0.02f, Color.White, 4f, -0.02f, 0f, 0f),
						_ => null
					};
					break;

				case 3:
					tempSprite = who.FarmerSprite.currentAnimationIndex switch
					{
						1 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(-32f, -64f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						2 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(-28f, -76f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						3 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 100f, 1, 0, who.Position + new Vector2(-16f, -128f), flicker: false,
							flipped: false, who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						4 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 200f, 1, 0, who.Position + new Vector2(0f, -124f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						5 => new TemporaryAnimatedSprite(Path.Combine("TileSheets", "weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16), 200f, 1, 0, who.Position + new Vector2(0f, -124f), flicker: false, flipped: false,
							who.getStandingY() / 10000f + 0.01f, 0.02f, Color.White, 4f, -0.02f, 0f, 0f),
						_ => null
					};
					break;
			}

			if ((toShow.Equals(who.ActiveObject) || (who.ActiveObject != null && toShow != null && toShow.ParentSheetIndex == who.ActiveObject.ParentSheetIndex)) && who.FarmerSprite.currentAnimationIndex == 5)
				tempSprite = null;

			if (tempSprite != null) who.currentLocation.temporarySprites.Add(tempSprite);

			if (who.FarmerSprite.currentAnimationIndex == 5)
			{
				who.Halt();
				who.FarmerSprite.CurrentAnimation = null;
			}

			return false; // don't run original logic
		}

		#endregion harmony patches
	}
}