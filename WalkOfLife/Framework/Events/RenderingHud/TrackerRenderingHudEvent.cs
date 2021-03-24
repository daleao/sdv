using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	internal class TrackerRenderingHudEvent : RenderingHudEvent
	{
		/// <summary>Construct an instance.</summary>
		internal TrackerRenderingHudEvent() { }

		/// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. Render pointers over trackable objects in view.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			foreach (var kvp in Game1.currentLocation.Objects.Pairs)
				if (Utility.ShouldPlayerTrackObject(kvp.Value)) Utility.DrawArrowPointerOverTarget(kvp.Key, Color.Yellow);
		}
	}
}
