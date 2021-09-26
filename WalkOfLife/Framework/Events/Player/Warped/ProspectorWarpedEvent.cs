using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.IO;
using TheLion.Stardew.Professions.Framework.TreasureHunt;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ProspectorWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			ModEntry.ProspectorHunt ??= new ProspectorHunt(ModEntry.ModHelper.Translation.Get("prospector.huntstarted"),
				ModEntry.ModHelper.Translation.Get("prospector.huntfailed"),
				ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "prospector.png")));

			if (ModEntry.ProspectorHunt.TreasureTile != null) ModEntry.ProspectorHunt.End();

			if (Game1.CurrentEvent == null && e.NewLocation is MineShaft) ModEntry.ProspectorHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}