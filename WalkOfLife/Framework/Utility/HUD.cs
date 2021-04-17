using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.IO;
using SUtility = StardewValley.Utility;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Holds common methods and properties related to rendering elements to the game HUD.</summary>
	public static partial class Utility
	{
		public static ArrowPointer ArrowPointer { get; set; }

		/// <summary>Draw a tracking arrow pointer on the edge of the screen pointing to a target off-screen.</summary>
		/// <param name="target">The target to point to.</param>
		/// <param name="color">The color of the pointer.</param>
		/// <remarks>Note that the game will add a yellow tinge to the color supplied here.</remarks>
		public static void DrawTrackingArrowPointer(Vector2 target, Color color)
		{
			if (SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			ArrowPointer ??= new ArrowPointer(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "cursor.png")));

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

			var srcRect = new Rectangle(0, 0, 5, 4);
			var renderScale = 4f;
			var safePos = SUtility.makeSafe(renderSize: new Vector2(srcRect.Width * renderScale, srcRect.Height * renderScale), renderPos: onScreenPosition);
			Game1.spriteBatch.Draw(ArrowPointer.Texture, safePos, srcRect, color, rotation, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
		}

		/// <summary>Draw a tracking arrow pointer over a target on-screen.</summary>
		/// <param name="target">A target on the game location.</param>
		/// <param name="color">The color of the pointer.</param>
		/// <remarks>Note that the game will add a yellow tinge to the color supplied here. Credit to Bpendragon for this logic.</remarks>
		public static void DrawArrowPointerOverTarget(Vector2 target, Color color)
		{
			if (!SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			ArrowPointer ??= new ArrowPointer(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "cursor.png")));

			var srcRect = new Rectangle(0, 0, 5, 4);
			const float renderScale = 4f;
			var targetPixel = new Vector2((target.X * 64f) + 32f, (target.Y * 64f) + 32f) + ArrowPointer.GetOffset();
			var adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
			adjustedPixel = SUtility.ModifyCoordinatesForUIScale(adjustedPixel);
			Game1.spriteBatch.Draw(ArrowPointer.Texture, adjustedPixel, srcRect, color, (float)Math.PI, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
		}
	}
}