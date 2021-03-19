using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	public abstract class TreasureHunt
	{
		public static Vector2? TreasureTile = null;
		protected static uint _elapsed = 0;

		protected readonly IMonitor _monitor;
		protected readonly Random _random;
		private readonly uint _timeLimit;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The overal mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		/// <param name="seed">Unique seed for RNG.</param>
		public TreasureHunt(ProfessionsConfig config, IMonitor monitor, int seed)
		{
			_timeLimit = config.TreasureHuntTimeLimitSeconds;
			_monitor = monitor;
			_random = new Random(seed);
		}

		/// <summary>Check for completion or failure on every update tick.</summary>
		/// <param name="ticks">The number of ticks elapsed since the game started.</param>
		public void Update(uint ticks)
		{
			if (ticks % 60 == 0 && ++_elapsed > _timeLimit) Fail();

			CheckForCompletion();
		}

		/// <summary>Try to start a new hunt at this location.</summary>
		/// <param name="location">The game location.</param>
		public abstract void TryStartNewHunt(GameLocation location);

		/// <summary>Check if the player has found the treasure tile.</summary>
		protected abstract void CheckForCompletion();

		/// <summary>End the hunt unsuccessfully.</summary>
		protected abstract void Fail();
	}
}
