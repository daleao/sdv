using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using SUtility = StardewValley.Utility;

namespace TheLion.AwesomeProfessions
{
	/// <summary>HUD message for treasure hunts.</summary>
	public class HuntNotification : HUDMessage
	{
		private Texture2D _Icon { get; }

		/// <summary>Construct an instance.</summary>
		/// <param name="message">The message to display.</param>
		public HuntNotification(string message)
			: base(message)
		{
			whatType = 0;
			noIcon = true;
			timeLeft = 5250f;
			fadeIn = false;
		}

		/// <summary>Construct an instance.</summary>
		/// <param name="message">The message to display.</param>
		/// <param name="icon">The icon to display.</param>
		public HuntNotification(string message, Texture2D icon)
			: base(message)
		{
			whatType = 0;
			noIcon = false;
			timeLeft = 5250f;
			fadeIn = true;
			_Icon = icon;
		}

		/// <summary>Draw the notification to the game sprite batch.</summary>
		public override void draw(SpriteBatch b, int i)
		{
			var titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
			if (noIcon)
			{
				var overrideX = titleSafeArea.Left + 16;
				var overrideY = ((Game1.uiViewport.Width < 1400) ? (-64) : 0) + titleSafeArea.Bottom - (i + 1) * 64 * 7 / 4 - 21 - (int)Game1.dialogueFont.MeasureString(message).Y;
				IClickableMenu.drawHoverText(b, message, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, overrideX, overrideY, transparency);
				return;
			}

			var itemBoxPosition = new Vector2(titleSafeArea.Left + 16, titleSafeArea.Bottom - (i + 1) * 64 * 7 / 4 - 64);
			if (Game1.isOutdoorMapSmallerThanViewport())
				itemBoxPosition.X = Math.Max(titleSafeArea.Left + 16, -Game1.uiViewport.X + 16);

			if (Game1.uiViewport.Width < 1400) itemBoxPosition.Y -= 48f;

			b.Draw(Game1.mouseCursors, itemBoxPosition, new Rectangle(293, 360, 26, 24), Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			var messageWidth = Game1.smallFont.MeasureString(message).X;
			b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 104f, itemBoxPosition.Y), new Rectangle(319, 360, 1, 24), Color.White * transparency, 0f, Vector2.Zero, new Vector2(messageWidth, 4f), SpriteEffects.None, 1f);
			b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 104f + messageWidth, itemBoxPosition.Y), new Rectangle(323, 360, 6, 24), Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			itemBoxPosition.X += 16f;
			itemBoxPosition.Y += 16f;
			b.Draw(_Icon, itemBoxPosition + new Vector2(8f, 8f) * 4f, new Rectangle(0, 0, 16, 16), Color.White * transparency, 0f, new Vector2(8f, 8f), 4f + Math.Max(0f, (timeLeft - 3000f) / 900f), SpriteEffects.None, 1f);
			itemBoxPosition.X += 51f;
			itemBoxPosition.Y += 51f;
			if (number > 1) SUtility.drawTinyDigits(number, b, itemBoxPosition, 3f, 1f, Color.White * transparency);

			itemBoxPosition.X += 32f;
			itemBoxPosition.Y -= 33f;
			SUtility.drawTextWithShadow(b, message, Game1.smallFont, itemBoxPosition, Game1.textColor * transparency, 1f, 1f, -1, -1, transparency);
		}
	}
}