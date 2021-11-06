using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using SUtility = StardewValley.Utility;

// ReSharper disable JoinDeclarationAndInitializer

namespace TheLion.Stardew.Professions.Framework.Utility
{
	/// <summary>Holds common methods and properties related to rendering elements to the game HUD.</summary>
	public static class HUD
	{
		private const int MAX_BAR_HEIGHT_I = 168, TEXTURE_HEIGHT_I = 46;
		private const float RENDER_SCALE_F = Game1.pixelZoom;
		public static ArrowPointer Pointer { get; set; }

		private static Texture2D BarTx { get; } =
			ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "hud", "bar.png"));

		/// <summary>Draw a tracking arrow pointer on the edge of the screen pointing to a target off-screen.</summary>
		/// <param name="target">The target to point to.</param>
		/// <param name="color">The color of the pointer.</param>
		public static void DrawTrackingArrowPointer(Vector2 target, Color color)
		{
			if (SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			Pointer ??= new();

			var vpbounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
			Vector2 onScreenPosition = default;
			var rotation = 0f;
			if (target.X * 64f > Game1.viewport.MaxCorner.X - 64)
			{
				onScreenPosition.X = vpbounds.Right - 8;
				rotation = (float) Math.PI / 2f;
			}
			else if (target.X * 64f < Game1.viewport.X)
			{
				onScreenPosition.X = 8f;
				rotation = -(float) Math.PI / 2f;
			}
			else
			{
				onScreenPosition.X = target.X * 64f - Game1.viewport.X;
			}

			if (target.Y * 64f > Game1.viewport.MaxCorner.Y - 64)
			{
				onScreenPosition.Y = vpbounds.Bottom - 8;
				rotation = (float) Math.PI;
			}
			else if (target.Y * 64f < Game1.viewport.Y)
			{
				onScreenPosition.Y = 8f;
			}
			else
			{
				onScreenPosition.Y = target.Y * 64f - Game1.viewport.Y;
			}

			if ((int) onScreenPosition.X == 8 && (int) onScreenPosition.Y == 8) rotation += (float) Math.PI / 4f;

			if ((int) onScreenPosition.X == 8 && (int) onScreenPosition.Y == vpbounds.Bottom - 8)
				rotation += (float) Math.PI / 4f;

			if ((int) onScreenPosition.X == vpbounds.Right - 8 && (int) onScreenPosition.Y == 8)
				rotation -= (float) Math.PI / 4f;

			if ((int) onScreenPosition.X == vpbounds.Right - 8 && (int) onScreenPosition.Y == vpbounds.Bottom - 8)
				rotation -= (float) Math.PI / 4f;

			var srcRect = new Rectangle(0, 0, 5, 4);
			var safePos = SUtility.makeSafe(
				renderSize: new(srcRect.Width * RENDER_SCALE_F, srcRect.Height * RENDER_SCALE_F),
				renderPos: onScreenPosition
			);

			Game1.spriteBatch.Draw(
				Pointer.Texture,
				safePos,
				srcRect,
				color,
				rotation,
				new(2f, 2f),
				RENDER_SCALE_F,
				SpriteEffects.None,
				1f
			);
		}

		/// <summary>Draw a tracking arrow pointer over a target on-screen.</summary>
		/// <param name="target">A target on the game location.</param>
		/// <param name="color">The color of the pointer.</param>
		/// <remarks>Credit to <c>Bpendragon</c>.</remarks>
		public static void DrawArrowPointerOverTarget(Vector2 target, Color color)
		{
			if (!SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			Pointer ??= new();

			var srcRect = new Rectangle(0, 0, 5, 4);
			var targetPixel = new Vector2(target.X * 64f + 32f, target.Y * 64f + 32f) + Pointer.GetOffset();
			var adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
			adjustedPixel = SUtility.ModifyCoordinatesForUIScale(adjustedPixel);

			Game1.spriteBatch.Draw(
				Pointer.Texture,
				adjustedPixel,
				srcRect,
				color,
				(float) Math.PI,
				new(2f, 2f),
				RENDER_SCALE_F,
				SpriteEffects.None,
				1f
			);
		}

		/// <summary>Draw the super gauge on the screen.</summary>
		public static void DrawSuperModeBar()
		{
			// get bar position
			var topOfBar = new Vector2(
				Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 56,
				Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 200
			);

			if (Game1.isOutdoorMapSmallerThanViewport())
				topOfBar.X = Math.Min(topOfBar.X,
					-Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 48);

			if (Game1.showingHealth) topOfBar.X -= 100;
			else topOfBar.X -= 44;

			// shake horizontally if full and on stand-by, if active also shake vertically
			if (ModEntry.IsSuperModeActive || ModEntry.ShouldShakeSuperModeBar)
			{
				topOfBar.X += Game1.random.Next(-3, 4);
				if (ModEntry.IsSuperModeActive) topOfBar.Y += Game1.random.Next(-3, 4);
			}

			// draw bar in thirds for flexibility
			Rectangle srcRect, destRect;

			// top
			srcRect = new(0, 0, 9, 16);
			Game1.spriteBatch.Draw(
				BarTx,
				topOfBar,
				srcRect,
				Color.White * ModEntry.SuperModeBarAlpha,
				0f,
				Vector2.Zero,
				RENDER_SCALE_F,
				SpriteEffects.None,
				1f
			);

			// middle
			srcRect = new(0, 16, 9, 16);
			destRect = new((int) topOfBar.X, (int) (topOfBar.Y + 64f), 36, 56);
			Game1.spriteBatch.Draw(
				BarTx,
				destRect,
				srcRect,
				Color.White * ModEntry.SuperModeBarAlpha
			);

			// bottom
			srcRect = new(0, 30, 9, 16);
			Game1.spriteBatch.Draw(
				BarTx,
				new(topOfBar.X, topOfBar.Y + 120f),
				srcRect,
				Color.White * ModEntry.SuperModeBarAlpha,
				0f,
				Vector2.Zero,
				RENDER_SCALE_F,
				SpriteEffects.None,
				1f
			);

			var ratio = ModEntry.SuperModeCounter / (float) ModEntry.SuperModeCounterMax;
			var srcHeight = (int) (TEXTURE_HEIGHT_I * ratio) - 2;
			var destHeight = (int) (MAX_BAR_HEIGHT_I * ratio);

			srcRect = new(10, TEXTURE_HEIGHT_I - srcHeight, 3, srcHeight);
			destRect = new((int) topOfBar.X + 12, (int) topOfBar.Y + 8 + (MAX_BAR_HEIGHT_I - destHeight), 12,
				destHeight);

			Game1.spriteBatch.Draw(
				BarTx,
				destRect,
				srcRect,
				Color.White,
				0f,
				Vector2.Zero,
				SpriteEffects.None,
				1f
			);

			// draw hover text
			if (Game1.getOldMouseX() >= topOfBar.X && Game1.getOldMouseY() >= topOfBar.Y &&
			    Game1.getOldMouseX() < topOfBar.X + 24f)
				Game1.drawWithBorder((int) Math.Max(0f, ModEntry.SuperModeCounter) + "/" + 500, Color.Black * 0f,
					Color.White,
					topOfBar + new Vector2(0f - Game1.dialogueFont.MeasureString("999/999").X - 32f, 64f));

			if (Math.Abs(ratio - 1f) >= 0.002f && !ModEntry.IsSuperModeActive) return;

			// draw top shadow
			destRect.Height = 2;
			Game1.spriteBatch.Draw(
				Game1.staminaRect,
				destRect,
				srcRect,
				Color.Black * 0.3f,
				0f,
				Vector2.Zero,
				SpriteEffects.None,
				1f
			);
		}
	}
}