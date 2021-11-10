using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class FarmerShowItemIntakePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmerShowItemIntakePatch()
		{
			Original = RequireMethod<Farmer>(nameof(Farmer.showItemIntake));
			Prefix = new(GetType().MethodNamed(nameof(FarmerShowItemIntakePrefix)));
		}

		#region harmony patches

		/// <summary>Patch to show weapons during crab pot harvest animation.</summary>
		[HarmonyPrefix]
		private static bool FarmerShowItemIntakePrefix(Farmer who)
		{
			try
			{
				if (!who.mostRecentlyGrabbedItem.ParentSheetIndex.IsAnyOf(14, 51)) return true; // run original logic

				var toShow = (SObject) who.mostRecentlyGrabbedItem;
				TemporaryAnimatedSprite tempSprite = who.FacingDirection switch
				{
					2 => who.FarmerSprite.currentAnimationIndex switch
					{
						1 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(0f, -32f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						2 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(0f, -43f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						3 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(0f, -128f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						4 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							200f, 1, 0, who.Position + new Vector2(0f, -120f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						5 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							200f, 1, 0, who.Position + new Vector2(0f, -120f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0.02f, Color.White, 4f, -0.02f, 0f, 0f),
						_ => null
					},
					1 => who.FarmerSprite.currentAnimationIndex switch
					{
						1 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(28f, -64f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						2 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(24f, -72f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						3 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(4f, -128f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						4 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							200f, 1, 0, who.Position + new Vector2(0f, -124f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						5 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							200f, 1, 0, who.Position + new Vector2(0f, -124f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0.02f, Color.White, 4f, -0.02f, 0f, 0f),
						_ => null
					},
					0 => who.FarmerSprite.currentAnimationIndex switch
					{
						1 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(0f, -32f), false, false,
							who.getStandingY() / 10000f - 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f),
						2 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(0f, -43f), false, false,
							who.getStandingY() / 10000f - 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f),
						3 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(0f, -128f), false, false,
							who.getStandingY() / 10000f - 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f),
						4 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							200f, 1, 0, who.Position + new Vector2(0f, -120f), false, false,
							who.getStandingY() / 10000f - 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f),
						5 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							200f, 1, 0, who.Position + new Vector2(0f, -120f), false, false,
							who.getStandingY() / 10000f - 0.001f, 0.02f, Color.White, 4f, -0.02f, 0f, 0f),
						_ => null
					},
					3 => who.FarmerSprite.currentAnimationIndex switch
					{
						1 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(-32f, -64f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						2 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(-28f, -76f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						3 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							100f, 1, 0, who.Position + new Vector2(-16f, -128f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						4 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							200f, 1, 0, who.Position + new Vector2(0f, -124f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0f, Color.White, 4f, 0f, 0f, 0f),
						5 => new(PathUtilities.NormalizeAssetName("TileSheets/weapons"),
							Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, toShow.ParentSheetIndex, 16,
								16),
							200f, 1, 0, who.Position + new Vector2(0f, -124f), false, false,
							who.getStandingY() / 10000f + 0.01f, 0.02f, Color.White, 4f, -0.02f, 0f, 0f),
						_ => null
					},
					_ => null
				};

				if ((toShow.Equals(who.ActiveObject) || who.ActiveObject is not null && toShow is not null &&
					    toShow.ParentSheetIndex == who.ActiveObject.ParentSheetIndex) &&
				    who.FarmerSprite.currentAnimationIndex == 5)
					tempSprite = null;

				if (tempSprite is not null) who.currentLocation.temporarySprites.Add(tempSprite);

				if (who.FarmerSprite.currentAnimationIndex != 5) return false; // don't run original logic

				who.Halt();
				who.FarmerSprite.CurrentAnimation = null;
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}