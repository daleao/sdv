using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeBarRenderingHudEvent : RenderingHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			Util.HUD.DrawSuperModeBar();
		}
	}
}