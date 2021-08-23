using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeBarRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			Util.HUD.DrawSuperModeBar();
		}
	}
}