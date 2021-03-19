using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System.Collections.Generic;
using System.IO;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	public class ScavengerHunt : TreasureHunt
	{
		private readonly HuntNotification _newHuntNotification;
		private readonly HuntNotification _failedHuntNotification;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The overal mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		/// <param name="i18n">Provides localized text.</param>
		/// <param name="content">Interface for loading content assets.</param>
		/// <param name="seed">Unique seed for RNG.</param>
		public ScavengerHunt(ProfessionsConfig config, IMonitor monitor, ITranslationHelper i18n, IContentHelper content, int seed)
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
			if (Utility.IsTileValidForPlacement(v, location))
			{
				Utility.MakeTileDiggable(v, location);
				TreasureTile = v;
				Game1.addHUDMessage(_newHuntNotification);
				_elapsed = 0;
				AwesomeProfessions.EventManager.Subscribe(new ScavengerHuntUpdateTickedEvent());
			}
		}

		/// <summary>Check if the player has found the treasure tile.</summary>
		protected override void CheckForCompletion()
		{
			if (Game1.player.CurrentTool is Hoe && Game1.player.UsingTool)
			{
				Vector2 actionTile = new Vector2((int)(Game1.player.GetToolLocation().X / Game1.tileSize), (int)(Game1.player.GetToolLocation().Y / Game1.tileSize));
				if (actionTile == TreasureTile.Value)
				{
					AwesomeProfessions.EventManager.Unsubscribe(typeof(ScavengerHuntUpdateTickedEvent));
					_BeginFindTreasure();
				}
			}
		}

		/// <summary>End the hunt unsuccessfully.</summary>
		protected override void Fail()
		{
			TreasureTile = null;
			Game1.addHUDMessage(_failedHuntNotification);
		}

		/// <summary>Play treasure chest found animation.</summary>
		private void _BeginFindTreasure()
		{
			Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Path.Combine("LooseSprites", "Cursors"), new Rectangle(64, 1920, 32, 32), 500f, 1, 0, Game1.player.Position + new Vector2(-32f, -160f), flicker: false, flipped: false, Game1.player.getStandingY() / 10000f + 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f)
			{
				motion = new Vector2(0f, -0.128f),
				timeBasedMotion = true,
				endFunction = _OpenChestEndFunction,
				extraInfoForEndBehavior = 0,
				alpha = 0f,
				alphaFade = -0.002f
			});
		}

		/// <summary>Play open treasure chest animation.</summary>
		/// <param name="extra">Not applicable.</param>
		private void _OpenChestEndFunction(int extra)
		{
			Game1.currentLocation.localSound("openChest");
			Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Path.Combine("LooseSprites", "Cursors"), new Rectangle(64, 1920, 32, 32), 200f, 4, 0, Game1.player.Position + new Vector2(-32f, -228f), flicker: false, flipped: false, Game1.player.getStandingY() / 10000f + 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f)
			{
				endFunction = _OpenTreasureMenuEndFunction,
				extraInfoForEndBehavior = 0
			});
		}

		/// <summary>Open the treasure chest menu.</summary>
		/// <param name="extra">Not applicable.</param>
		private void _OpenTreasureMenuEndFunction(int extra)
		{
			Game1.player.completelyStopAnimatingOrDoingAction();
			var treasures = _GetTreasureContents();
			Game1.activeClickableMenu = new ItemGrabMenu(treasures).setEssential(essential: true);
			(Game1.activeClickableMenu as ItemGrabMenu).source = 3;
			TreasureTile = null;
		}

		/// <summary>Choose contents of the treasure chest.</summary>
		private List<Item> _GetTreasureContents()
		{
			List<Item> treasures = new();
			treasures.Add(new SObject(166, 1));
			return treasures;
		}
	}
}