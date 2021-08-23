using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeBarFlashUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (e.Ticks % 6 == 0) Util.HUD.FlashColor = Color.Cyan;
			else if (e.Ticks % 6 == 3) Util.HUD.FlashColor = Color.Yellow;
		}
	}
}