using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using System;
using System.IO;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	internal class CrabPotDrawPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
				prefix: new HarmonyMethod(GetType(), nameof(CrabPotDrawPrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch to draw weapons in Luremaster crabpots.</summary>
		private static bool CrabPotDrawPrefix(ref CrabPot __instance, ref float ___yBob, ref Vector2 ___shake, SpriteBatch spriteBatch, int x, int y)
		{
			try
			{
				if (!__instance.readyForHarvest.Value || __instance.heldObject.Value == null || !__instance.heldObject.Value.ParentSheetIndex.AnyOf(14, 51))
					return true; // run original logic

				___yBob = (float)(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 500.0 + (x * 64)) * 8.0 + 8.0);
				if (___yBob <= 0.001f)
					Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(Path.Combine("TileSheets", "animations"), new Rectangle(0, 0, 64, 64), 150f, 8, 0, __instance.directionOffset.Value + new Vector2(x * 64f + 4f, y * 64f + 32f), flicker: false, Game1.random.NextDouble() < 0.5, 0.001f, 0.01f, Color.White, 0.75f, 0.003f, 0f, 0f));

				_ = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y];
				spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, __instance.directionOffset.Value + new Vector2(x * 64f, y * 64f + ___yBob)) + ___shake, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, __instance.tileIndexToShow, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + __instance.directionOffset.Value.Y + (x % 4)) / 10000f);
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, __instance.directionOffset.Value + new Vector2(x * 64f + 4f, y * 64f + 48f)) + ___shake, new Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2112 + ((x + y) % 2 != 0 ? (!Game1.currentLocation.waterTileFlip ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 128 : 0)), 56, 16 + (int)___yBob), Game1.currentLocation.waterColor.Value, 0f, Vector2.Zero, 1f, SpriteEffects.None, (y * 64 + __instance.directionOffset.Value.Y + (x % 4)) / 9999f);
				var yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, __instance.directionOffset.Value + new Vector2(x * 64f - 8f, (y * 64f - 96f - 16f) + yOffset)), new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, (y + 1) * 64 / 10000f + 1E-06f + __instance.TileLocation.X / 10000f);
				spriteBatch.Draw(Tool.weaponsTexture, Game1.GlobalToLocal(Game1.viewport, __instance.directionOffset.Value + new Vector2(x * 64f + 32f, (y * 64f - 64f - 8f) + yOffset)), Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, __instance.heldObject.Value.ParentSheetIndex, 16, 16), Color.White * 0.75f, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (y + 1) * 64 / 10000f + 1E-05f + __instance.TileLocation.X / 10000f);
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(CrabPotDrawPrefix)}:\n{ex}");
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}