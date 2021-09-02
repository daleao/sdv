using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.IO;
using TheLion.Stardew.Common.Classes;
using DrawColor = System.Drawing.Color;
using SUtility = StardewValley.Utility;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace TheLion.Stardew.Professions.Framework.Util
{
	/// <summary>Holds common methods and properties related to rendering elements to the game HUD.</summary>
	public static class HUD
	{
		public static ArrowPointer Pointer { get; set; }

		private const int MAX_BAR_HEIGHT = 168, TEXTURE_HEIGHT = 46;
		private const float RENDER_SCALE = Game1.pixelZoom;

		private static readonly Texture2D _barTx = ModEntry.Content.Load<Texture2D>(Path.Combine("assets", "hud", "bar.png"));
		private static Texture2D _barFillTx;

		/// <summary>Draw a tracking arrow pointer on the edge of the screen pointing to a target off-screen.</summary>
		/// <param name="target">The target to point to.</param>
		/// <param name="color">The color of the pointer.</param>
		public static void DrawTrackingArrowPointer(Vector2 target, XnaColor color)
		{
			if (SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			Pointer ??= new ArrowPointer();

			var vpbounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
			Vector2 onScreenPosition = default;
			var rotation = 0f;
			if (target.X * 64f > Game1.viewport.MaxCorner.X - 64)
			{
				onScreenPosition.X = vpbounds.Right - 8;
				rotation = (float)Math.PI / 2f;
			}
			else if (target.X * 64f < Game1.viewport.X)
			{
				onScreenPosition.X = 8f;
				rotation = -(float)Math.PI / 2f;
			}
			else
			{
				onScreenPosition.X = target.X * 64f - Game1.viewport.X;
			}

			if (target.Y * 64f > Game1.viewport.MaxCorner.Y - 64)
			{
				onScreenPosition.Y = vpbounds.Bottom - 8;
				rotation = (float)Math.PI;
			}
			else if (target.Y * 64f < Game1.viewport.Y)
			{
				onScreenPosition.Y = 8f;
			}
			else
			{
				onScreenPosition.Y = target.Y * 64f - Game1.viewport.Y;
			}

			if ((int)onScreenPosition.X == 8 && (int)onScreenPosition.Y == 8) rotation += (float)Math.PI / 4f;

			if ((int)onScreenPosition.X == 8 && (int)onScreenPosition.Y == vpbounds.Bottom - 8) rotation += (float)Math.PI / 4f;

			if ((int)onScreenPosition.X == vpbounds.Right - 8 && (int)onScreenPosition.Y == 8) rotation -= (float)Math.PI / 4f;

			if ((int)onScreenPosition.X == vpbounds.Right - 8 && (int)onScreenPosition.Y == vpbounds.Bottom - 8) rotation -= (float)Math.PI / 4f;

			var srcRect = new XnaRect(0, 0, 5, 4);
			var safePos = SUtility.makeSafe(
				renderSize: new Vector2(srcRect.Width * RENDER_SCALE, srcRect.Height * RENDER_SCALE),
				renderPos: onScreenPosition
			);

			Game1.spriteBatch.Draw(
				texture: Pointer.Texture,
				position: safePos,
				sourceRectangle: srcRect,
				color: color,
				rotation: rotation,
				origin: new Vector2(2f, 2f),
				scale: RENDER_SCALE,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);
		}

		/// <summary>Draw a tracking arrow pointer over a target on-screen.</summary>
		/// <param name="target">A target on the game location.</param>
		/// <param name="color">The color of the pointer.</param>
		/// <remarks>Note that the game will add a yellow tinge to the color supplied here. Credit to Bpendragon for this logic.</remarks>
		public static void DrawArrowPointerOverTarget(Vector2 target, XnaColor color)
		{
			if (!SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			Pointer ??= new ArrowPointer();

			var srcRect = new XnaRect(0, 0, 5, 4);
			var targetPixel = new Vector2(target.X * 64f + 32f, target.Y * 64f + 32f) + Pointer.GetOffset();
			var adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
			adjustedPixel = SUtility.ModifyCoordinatesForUIScale(adjustedPixel);

			Game1.spriteBatch.Draw(
				texture: Pointer.Texture,
				position: adjustedPixel,
				sourceRectangle: srcRect,
				color: color,
				rotation: (float)Math.PI,
				origin: new Vector2(2f, 2f),
				scale: RENDER_SCALE,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);
		}

		/// <summary>Draw the super gauge on the screen.</summary>
		public static void DrawSuperModeBar()
		{
			// get bar position
			var topOfBar = new Vector2(
				x: Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 56,
				y: Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 200
			);

			if (Game1.isOutdoorMapSmallerThanViewport())
				topOfBar.X = Math.Min(topOfBar.X, -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 48);

			if (Game1.showingHealth) topOfBar.X -= 100;
			else topOfBar.X -= 44;

			// shake horizontally if full and on stand-by, if active also shake vertically
			if (ModEntry.IsSuperModeActive || ModEntry.ShouldShakeSuperModeBar)
			{
				topOfBar.X += Game1.random.Next(-3, 4);
				if (ModEntry.IsSuperModeActive) topOfBar.Y += Game1.random.Next(-3, 4);
			}

			// draw bar in thirds for flexibility
			XnaRect srcRect, destRect;

			// top
			srcRect = new XnaRect(0, 0, 9, 16);
			Game1.spriteBatch.Draw(
				texture: _barTx,
				position: topOfBar,
				sourceRectangle: srcRect,
				color: XnaColor.White * ModEntry.SuperModeBarOpacity,
				rotation: 0f,
				origin: Vector2.Zero,
				scale: RENDER_SCALE,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);

			// middle
			srcRect = new XnaRect(0, 16, 9, 16);
			destRect = new XnaRect((int)topOfBar.X, (int)(topOfBar.Y + 64f), 36, 56);
			Game1.spriteBatch.Draw(
				texture: _barTx,
				destinationRectangle: destRect,
				sourceRectangle: srcRect,
				color: XnaColor.White * ModEntry.SuperModeBarOpacity
			 );

			// bottom
			srcRect = new XnaRect(0, 30, 9, 16);
			Game1.spriteBatch.Draw(
				texture: _barTx,
				position: new Vector2(topOfBar.X, topOfBar.Y + 120f),
				sourceRectangle: srcRect,
				color: XnaColor.White * ModEntry.SuperModeBarOpacity,
				rotation: 0f,
				origin: Vector2.Zero,
				scale: RENDER_SCALE,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);

			// draw meter overlay
			if (_barFillTx == null)
			{
				var colors = new[]
				{
					DrawColor.Yellow,
					DrawColor.OrangeRed,
					DrawColor.MediumVioletRed,
					DrawColor.BlueViolet,
					DrawColor.Cyan
				};
				var positions = new[] { 0f, 0.35f, 0.55f, 0.75f, 1f };

				_barFillTx = TextureBuilder.CreateGradientTexture(Game1.graphics.GraphicsDevice, 9, TEXTURE_HEIGHT, colors, positions);
			}

			var ratio = ModEntry.SuperModeCounter / (float)ModEntry.SuperModeCounterMax;
			var srcHeight = (int)(TEXTURE_HEIGHT * ratio);
			var destHeight = (int)(MAX_BAR_HEIGHT * ratio);

			srcRect = new XnaRect(0, TEXTURE_HEIGHT - srcHeight, 9, srcHeight);
			destRect = new XnaRect((int)topOfBar.X + 12, (int)topOfBar.Y + 8 + (MAX_BAR_HEIGHT - destHeight), 12, destHeight);

			Game1.spriteBatch.Draw(
				texture: _barFillTx,
				destinationRectangle: destRect,
				sourceRectangle: srcRect,
				color: XnaColor.White,
				rotation: 0f,
				origin: Vector2.Zero,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);

			// draw hover text
			if (Game1.getOldMouseX() >= topOfBar.X && Game1.getOldMouseY() >= topOfBar.Y && Game1.getOldMouseX() < topOfBar.X + 24f)
				Game1.drawWithBorder((int)Math.Max(0f, ModEntry.SuperModeCounter) + "/" + 500, XnaColor.Black * 0f, XnaColor.White, topOfBar + new Vector2(0f - Game1.dialogueFont.MeasureString("999/999").X - 32f, 64f));

			if (Math.Abs(ratio - 1f) >= 0.002f && !ModEntry.IsSuperModeActive) return;

			// draw top shadow
			destRect.Height = 2;
			Game1.spriteBatch.Draw(
				texture: Game1.staminaRect,
				destinationRectangle: destRect,
				sourceRectangle: srcRect,
				color: XnaColor.Black * 0.3f,
				rotation: 0f,
				origin: Vector2.Zero,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);
		}
	}
}