using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace TheLion.Stardew.Professions.Framework.Util
{
	/// <summary>Vertical arrow indicator to reveal on-screen objects of interest for tracker professions.</summary>
	public class ArrowPointer
	{
		public Texture2D Texture { get; } = ModEntry.Content.Load<Texture2D>(Path.Combine("assets", "hud", "pointer.png"));

		private const float MAX_STEP = 3f, MIN_STEP = -3f;

		private float _height = -42f, _jerk = 1f, _step;

		/// <summary>Advance the pointer's vertical offset motion by one step, in a bobbing fashion.</summary>
		public void Bob()
		{
			if (_step == MAX_STEP || _step == MIN_STEP) _jerk = -_jerk;
			_step += _jerk;
			_height += _step;
		}

		/// <summary>Get the pointer's current vertical offset.</summary>
		public Vector2 GetOffset()
		{
			return new Vector2(0f, _height);
		}
	}
}