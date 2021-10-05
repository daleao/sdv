using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DrawColor = System.Drawing.Color;
using DrawRect = System.Drawing.Rectangle;

namespace TheLion.Stardew.Common.Classes
{
	public static class TextureBuilder
	{
		/// <summary>Create a rectangular texture and fill it with a vertical color gradient.</summary>
		/// <param name="device">The game's <see cref="GraphicsDevice"/>.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <param name="colors">Array of <see cref="DrawColor"/>s.</param>
		/// <param name="positions">Array of floats between 0 and 1, representing the positions of the colors.</param>
		public static Texture2D CreateGradientTexture(GraphicsDevice device, int width, int height, DrawColor[] colors,
			float[] positions)
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
	}
}