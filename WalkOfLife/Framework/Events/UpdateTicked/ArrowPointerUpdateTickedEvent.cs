using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using System.IO;

namespace TheLion.AwesomeProfessions
{
	internal class ArrowPointerUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			Utility.ArrowPointer ??= new ArrowPointer(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "cursor.png")));
			if (e.Ticks % 4 == 0) Utility.ArrowPointer.Bob();
		}
	}
}