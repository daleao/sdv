using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using System.IO;
using TheLion.Stardew.Professions.Framework.TreasureHunt;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ScavengerWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			ModEntry.ScavengerHunt ??= new ScavengerHunt(ModEntry.I18n.Get("scavenger.huntstarted"),
				ModEntry.I18n.Get("scavenger.huntfailed"),
				ModEntry.Content.Load<Texture2D>(Path.Combine("assets", "scavenger.png")));

			if (ModEntry.ScavengerHunt.TreasureTile != null) ModEntry.ScavengerHunt.End();

			if (Game1.CurrentEvent == null && e.NewLocation.IsOutdoors && !(e.NewLocation.IsFarm || e.NewLocation.NameOrUniqueName.Equals("Town")))
				ModEntry.ScavengerHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}