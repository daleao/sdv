using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DrawColor = System.Drawing.Color;
using DrawRect = System.Drawing.Rectangle;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace TheLion.Stardew.Common.Classes
{
	public static class TextureBuilder
	{
		/// <summary>Create a rectangular texture and fill it with a color gradient.</summary>
		/// <param name="device">The game's <see cref="GraphicsDevice"/>.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <param name="colors">Array of <see cref="DrawColor"/>s.</param>
		/// <param name="positions">Array of floats between 0 and 1, representing the positions of the colors.</param>
		public static Texture2D CreateRectangularGradient(GraphicsDevice device, int width, int height, DrawColor[] colors, float[] positions)
		{
			// define rectangle shape
			var rect = new DrawRect(0, 0, width, height);

			// create gradient bitmap
			using var bmp = new Bitmap(width, height);
			using var graphics = Graphics.FromImage(bmp);
			using var brush = new LinearGradientBrush(
				rect,
				colors[0],
				colors[colors.Length - 1],
				LinearGradientMode.Vertical
			);
			var blend = new ColorBlend
			{
				Colors = colors,
				Positions = positions
			};
			brush.InterpolationColors = blend;
			graphics.FillRectangle(brush, rect);

			// copy bitmap data to texture
			var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
			var tx = new Texture2D(device, bmp.Width, bmp.Height, false, SurfaceFormat.Color);
			var bufferSize = data.Height * data.Stride;
			var bytes = new byte[bufferSize];
			Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
			tx.SetData(bytes);
			bmp.UnlockBits(data);

			return tx;
		}

		/// <summary>Create a rectangular texture with rounded corners and a flat color.</summary>
		/// <param name="device">The game's <see cref="GraphicsDevice"/>.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <param name="cornerRadius">The radius of the corners of the rectangle.</param>
		/// <param name="color"><see cref="XnaColor"/> to fill the texture.</param>
		public static Texture2D CreateRoundedRectangle(GraphicsDevice device, int width, int height, int cornerRadius, XnaColor color)
		{
			var tx = new Texture2D(device, width, height, false, SurfaceFormat.Color);
			var colors = new XnaColor[width * height];
			var internalRect = new XnaRect(cornerRadius, cornerRadius, width - 2 * cornerRadius, height - 2 * cornerRadius);
			for (var x = 0; x < tx.Width; ++x)
			{
				for (var y = 0; y < tx.Height; ++y)
				{
					if (internalRect.Contains(x, y))
					{
						colors[x + width * y] = color;
						continue;
					}

					var point = new Vector2(x, y);
					var origin = Vector2.Zero;
					if (x < cornerRadius)
					{
						if (y < cornerRadius) origin = new Vector2(cornerRadius, cornerRadius);
						else if (y > height - cornerRadius) origin = new Vector2(cornerRadius, height - cornerRadius);
						else origin = new Vector2(cornerRadius, y);
					}
					else if (x > width - cornerRadius)
					{
						if (y < cornerRadius) origin = new Vector2(width - cornerRadius, cornerRadius);
						else if (y > height - cornerRadius) origin = new Vector2(width - cornerRadius, height - cornerRadius);
						else origin = new Vector2(width - cornerRadius, y);
					}
					else
					{
						if (y < cornerRadius) origin = new Vector2(x, cornerRadius);
						else if (y > height - cornerRadius) origin = new Vector2(x, height - cornerRadius);
					}

					if (origin.Equals(Vector2.Zero)) continue;

					var distance = Vector2.Distance(point, origin);
					if (distance > cornerRadius + 1) colors[x + width * y] = XnaColor.Transparent;
				}
			}

			tx.SetData(colors);
			return tx;
		}
	}
}