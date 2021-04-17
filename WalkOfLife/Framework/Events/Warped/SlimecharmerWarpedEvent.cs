using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class SlimecharmerWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (e.NewLocation is not MineShaft && e.NewLocation is not VolcanoDungeon) return;

			var count = Game1.getFarm().buildings.Where(b => (b.owner.Value == e.Player.UniqueMultiplayerID || !Game1.IsMultiplayer) && b.indoors.Value is SlimeHutch && !b.isUnderConstruction() && b.indoors.Value.characters.Any()).Sum(b => b.indoors.Value.characters.Count(npc => npc is GreenSlime)) + Game1.getFarm().characters.Count(npc => npc is GreenSlime);
			var r = new Random(Guid.NewGuid().GetHashCode());
			while (count-- > 0)
			{
				var x = r.Next(e.NewLocation.Map.GetLayer("Back").LayerWidth);
				var y = r.Next(e.NewLocation.Map.GetLayer("Back").LayerHeight);
				var spawnPosition = new Vector2(x, y);

				GreenSlime slime;
				switch (e.NewLocation)
				{
					case MineShaft shaft when e.NewLocation is MineShaft:
						{
							shaft.checkForMapAlterations(x, y);
							if (!shaft.isTileClearForMineObjects(spawnPosition) || shaft.isTileOccupied(spawnPosition)) continue;

							slime = new GreenSlime(Vector2.Zero, shaft.mineLevel);
							if (shaft.GetAdditionalDifficulty() > 0 && r.NextDouble() < Math.Min(shaft.GetAdditionalDifficulty() * 0.1f, 0.5f))
							{
								slime.stackedSlimes.Value = r.NextDouble() < 0.0099999997764825821 ? 4 : 2;
							}

							slime.setTilePosition(x, y);
							shaft.characters.Add(shaft.BuffMonsterIfNecessary(slime));
							break;
						}
					case VolcanoDungeon dungeon when e.NewLocation is VolcanoDungeon:
						{
							if (!e.NewLocation.isTileLocationTotallyClearAndPlaceable(spawnPosition)) continue;

							slime = new GreenSlime(spawnPosition, 1);
							slime.makeTigerSlime();
							dungeon.characters.Add(slime);
							break;
						}
				}
			}
		}
	}
}