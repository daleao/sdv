using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Framework.Util;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeBarRenderingHudEvent : RenderingHudEvent
	{
		/// <inheritdoc />
		public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			HUD.DrawSuperModeBar();
		}
	}
}