using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	public class ProspectorHunt : TreasureHunt
	{
		private readonly HuntNotification _newHuntNotification;
		private readonly HuntNotification _failedHuntNotification;
		
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The overal mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		/// <param name="i18n">Provides localized text.</param>
		/// <param name="content">Interface for loading content assets.</param>
		/// <param name="seed">Unique seed for RNG.</param>
		public ProspectorHunt(ProfessionsConfig config, IMonitor monitor, ITranslationHelper i18n, IContentHelper content, int seed)
			: base(config, monitor, seed)
		{
			_newHuntNotification = new HuntNotification(i18n.Get("scavenger.huntstarted"), content.Load<Texture2D>(Path.Combine("Assets", "scavenger.png")));
			_failedHuntNotification = new HuntNotification(i18n.Get("scavenger.huntfailed"));
		}

		/// <summary>Try to start a new hunt at this location.</summary>
		/// <param name="location">The game location.</param>
		public override void TryStartNewHunt(GameLocation location)
		{
			int x = _random.Next(location.Map.DisplayWidth / Game1.tileSize);
			int y = _random.Next(location.Map.DisplayHeight / Game1.tileSize);
			Vector2 v = new Vector2(x, y);
			if (Utility.DoesTileContainStone(v, location))
			{
				TreasureTile = v;
				Game1.addHUDMessage(_newHuntNotification);
				_elapsed = 0;
				AwesomeProfessions.EventManager.Subscribe(new ProspectorHuntUpdateTickedEvent());
			}
		}

		/// <summary>Check if the player has found the treasure tile.</summary>
		protected override void CheckForCompletion()
		{
			if (!Game1.currentLocation.Objects.ContainsKey(TreasureTile.Value))
			{
				TreasureTile = null;
				AwesomeProfessions.EventManager.Unsubscribe(typeof(ProspectorHuntUpdateTickedEvent));
			}
		}

		/// <summary>End the hunt unsuccessfully.</summary>
		protected override void Fail()
		{
			TreasureTile = null;
			Game1.addHUDMessage(_failedHuntNotification);
		}
	}
}