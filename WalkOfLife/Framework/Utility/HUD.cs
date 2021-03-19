using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using SUtility = StardewValley.Utility;

namespace TheLion.AwesomeProfessions
{
	public static partial class Utility
	{
		public static ArrowPointer ArrowPointer { get; set; } = new();

		/// <summary>Draw a tracking arrow pointer on the edge of the screen pointing to a target off-screen.</summary>
		/// <param name="target">The target to point to.</param>
		/// <param name="color">The color of the pointer.</param>
		/// <remarks>Note that the game will add a yellow tinge to the color supplied here.</remarks>
		public static void DrawTrackingArrowPointer(Vector2 target, Color color)
		{
			if (SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			Rectangle vpbounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
			Vector2 onScreenPosition = default;
			float rotation = 0f;
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
				onScreenPosition.X = target.X * 64f - Game1.viewport.X;

			if (target.Y * 64f > Game1.viewport.MaxCorner.Y - 64)
			{
				onScreenPosition.Y = vpbounds.Bottom - 8;
				rotation = (float)Math.PI;
			}
			else if (target.Y * 64f < Game1.viewport.Y)
				onScreenPosition.Y = 8f;
			else
				onScreenPosition.Y = target.Y * 64f - Game1.viewport.Y;

			if (onScreenPosition.X == 8f && onScreenPosition.Y == 8f)
				rotation += (float)Math.PI / 4f;

			if (onScreenPosition.X == 8f && onScreenPosition.Y == vpbounds.Bottom - 8)
				rotation += (float)Math.PI / 4f;

			if (onScreenPosition.X == vpbounds.Right - 8 && onScreenPosition.Y == 8f)
				rotation -= (float)Math.PI / 4f;

			if (onScreenPosition.X == vpbounds.Right - 8 && onScreenPosition.Y == vpbounds.Bottom - 8)
				rotation -= (float)Math.PI / 4f;

			Rectangle srcRect = new Rectangle(0, 0, 5, 4);
			float renderScale = 4f;
			Vector2 safePos = SUtility.makeSafe(renderSize: new Vector2(srcRect.Width * renderScale, srcRect.Height * renderScale), renderPos: onScreenPosition);
			Game1.spriteBatch.Draw(ArrowPointer.Texture, safePos, srcRect, color, rotation, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
		}

		/// <summary>Draw a tracking arrow pointer on the edge of the screen pointing to an off-screen target.</summary>
		/// <param name="target">The target to point to.</param>
		/// <param name="color">The color of the pointer.</param>
		/// <param name="spriteBatch">The sprite batch to draw to.</param>
		/// <remarks>Note that the game will add a yellow tinge to the color supplied here.</remarks>
		public static void DrawTrackingArrowPointer(Vector2 target, Color color, SpriteBatch spriteBatch)
		{
			if (SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			Rectangle vpbounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
			Vector2 onScreenPosition = default;
			float rotation = 0f;
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
				onScreenPosition.X = target.X * 64f - Game1.viewport.X;

			if (target.Y * 64f > Game1.viewport.MaxCorner.Y - 64)
			{
				onScreenPosition.Y = vpbounds.Bottom - 8;
				rotation = (float)Math.PI;
			}
			else if (target.Y * 64f < Game1.viewport.Y)
				onScreenPosition.Y = 8f;
			else
				onScreenPosition.Y = target.Y * 64f - Game1.viewport.Y;

			if (onScreenPosition.X == 8f && onScreenPosition.Y == 8f)
				rotation += (float)Math.PI / 4f;

			if (onScreenPosition.X == 8f && onScreenPosition.Y == vpbounds.Bottom - 8)
				rotation += (float)Math.PI / 4f;

			if (onScreenPosition.X == vpbounds.Right - 8 && onScreenPosition.Y == 8f)
				rotation -= (float)Math.PI / 4f;

			if (onScreenPosition.X == vpbounds.Right - 8 && onScreenPosition.Y == vpbounds.Bottom - 8)
				rotation -= (float)Math.PI / 4f;

			Rectangle srcRect = new Rectangle(0, 0, 5, 4);
			float renderScale = 4f;
			Vector2 safePos = SUtility.makeSafe(renderSize: new Vector2(srcRect.Width * renderScale, srcRect.Height * renderScale), renderPos: onScreenPosition);
			spriteBatch.Draw(ArrowPointer.Texture, safePos, srcRect, color, rotation, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
		}

		/// <summary>Draw a tracking arrow pointer over a target on-screen.</summary>
		/// <param name="target">A target on the game location.</param>
		/// <param name="color">The color of the pointer.</param>
		/// <remarks>Note that the game will add a yellow tinge to the color supplied here. Credit to Bpendragon for this logic.</remarks>
		public static void DrawArrowPointerOverTarget(Vector2 target, Color color)
		{
			if (!SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			Rectangle srcRect = new Rectangle(0, 0, 5, 4);
			float renderScale = 4f;
			Vector2 targetPixel = new Vector2((target.X * 64f) + 32f, (target.Y * 64f) + 32f) + ArrowPointer.GetOffset();
			Vector2 adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
			adjustedPixel = SUtility.ModifyCoordinatesForUIScale(adjustedPixel);
			Game1.spriteBatch.Draw(ArrowPointer.Texture, adjustedPixel, srcRect, color, (float)Math.PI, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
		}

		/// <summary>Draw a tracking arrow pointer over a target on-screen.</summary>
		/// <param name="target">A target on the game location.</param>
		/// <param name="color">The color of the pointer.</param>
		/// <param name="spriteBatch">The sprite batch to draw to.</param>
		/// <remarks>Note that the game will add a yellow tinge to the color supplied here. Credit to Bpendragon for this logic.</remarks>
		public static void DrawArrowPointerOverTarget(Vector2 target, Color color, SpriteBatch spriteBatch)
		{
			if (!SUtility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

			Rectangle srcRect = new Rectangle(0, 0, 5, 4);
			float renderScale = 4f;
			Vector2 targetPixel = new Vector2((target.X * 64f) + 32f, (target.Y * 64f) + 32f) + ArrowPointer.GetOffset();
			Vector2 adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
			adjustedPixel = SUtility.ModifyCoordinatesForUIScale(adjustedPixel);
			spriteBatch.Draw(ArrowPointer.Texture, adjustedPixel, srcRect, color, (float)Math.PI, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
		}
	}

	public class ArrowPointer
	{
		public Texture2D Texture { get; set; }
		private float _height = -42f, _step = 0f, _maxStep = 3f, _minStep = -3f, _jerk = 1f;

		public void Bob()
		{
			if (_step == _maxStep || _step == _minStep) _jerk = -_jerk;

			_step += _jerk;
			_height += _step;
		}

		public Vector2 GetOffset()
		{
			return new Vector2(0f, _height);
		}
	}
}
