using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal abstract class TreasureHunt
	{
		public Vector2? TreasureTile { get; protected set; } = null;

		protected Random Random { get; } = new Random(Guid.NewGuid().GetHashCode());
		protected EventManager Manager { get; }
		protected ProfessionsData Data { get; }
		protected string NewHuntMessage { get; set; }
		protected string FailedHuntMessage { get; set; }
		protected Texture2D Icon { get; set; }
		protected uint TimeLimit { private get; set; }
		
		private double _BaseTriggerChance { get; }
		
		protected uint _elapsed = 0;
		private double _accumulatedBonus = 1.0;


		/// <summary>Construct an instance.</summary>
		/// <param name="config">The overal mod settings.</param>
		protected TreasureHunt(ProfessionsConfig config, ProfessionsData data, EventManager manager)
		{
			Data = data;
			Manager = manager;
			_BaseTriggerChance = config.ChanceToStartTreasureHunt;
		}

		/// <summary>Check for completion or failure on every update tick.</summary>
		/// <param name="ticks">The number of ticks elapsed since the game started.</param>
		internal void Update(uint ticks)
		{
			if (ticks % 60 == 0 && ++_elapsed > TimeLimit) Fail();

			CheckForCompletion();
		}

		/// <summary>Reset the accumulated bonus chance to trigger a new hunt.</summary>
		internal void ResetAccumulatedBonus()
		{
			_accumulatedBonus = 1.0;
		}

		/// <summary>Start a new treasure hunt or adjust the odds for the next attempt.</summary>
		protected bool TryStartNewHunt()
		{
			if (Random.NextDouble() > _BaseTriggerChance * _accumulatedBonus)
			{
				_accumulatedBonus *= 1.0 + Game1.player.DailyLuck;
				return false;
			}

			_accumulatedBonus = 1.0;
			return true;
		}

		/// <summary>Try to start a new hunt at this location.</summary>
		/// <param name="location">The game location.</param>
		internal abstract void TryStartNewHunt(GameLocation location);

		/// <summary>Reset treasure tile and unsubscribe treasure hunt update event.</summary>
		internal abstract void End();

		/// <summary>Check if the player has found the treasure tile.</summary>
		protected abstract void CheckForCompletion();

		/// <summary>End the hunt unsuccessfully.</summary>
		protected abstract void Fail();
	}
}
