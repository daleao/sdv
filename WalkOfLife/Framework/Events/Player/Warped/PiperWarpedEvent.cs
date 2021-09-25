using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class PiperWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;
			
			if(e.NewLocation is not VolcanoDungeon && (e.NewLocation is not MineShaft || (e.NewLocation as MineShaft).IsTreasureOrSafeRoom()))
			{
				ModEntry.Subscriber.Unsubscribe(typeof(PiperUpdateTickedEvent));
				return;
			}

			var attempts = Util.Professions.GetPiperSlimeSpawnAttempts();
			var spawned = 0;
			var r = new Random(Guid.NewGuid().GetHashCode());
			while (attempts-- > 0 || spawned < 1)
			{
				var x = r.Next(e.NewLocation.Map.GetLayer("Back").LayerWidth);
				var y = r.Next(e.NewLocation.Map.GetLayer("Back").LayerHeight);
				var spawnPosition = new Vector2(x, y);

				GreenSlime slime;
				switch (e.NewLocation)
				{
					case MineShaft shaft:
						shaft.checkForMapAlterations(x, y);
						if (!shaft.isTileClearForMineObjects(spawnPosition) || shaft.isTileOccupied(spawnPosition)) continue;

						slime = new GreenSlime(Vector2.Zero, shaft.mineLevel);
						if (shaft.GetAdditionalDifficulty() > 0 && r.NextDouble() < Math.Min(shaft.GetAdditionalDifficulty() * 0.1f, 0.5f))
						{
							slime.stackedSlimes.Value = r.NextDouble() < 0.0099999997764825821 ? 4 : 2;
						}

						slime.setTilePosition(x, y);
						shaft.characters.Add(shaft.BuffMonsterIfNecessary(slime));
						++spawned;
						break;
					case VolcanoDungeon dungeon:
						if (!e.NewLocation.isTileLocationTotallyClearAndPlaceable(spawnPosition)) continue;

						slime = new GreenSlime(spawnPosition, 1);
						slime.makeTigerSlime();
						dungeon.characters.Add(slime);
						++spawned;
						break;
				}
				--attempts;
			}
			ModEntry.Log($"Spawned {spawned} Slimes after {attempts} attempts.", LogLevel.Trace);

			ModEntry.Subscriber.Subscribe(new PiperUpdateTickedEvent());
		}
	}
}