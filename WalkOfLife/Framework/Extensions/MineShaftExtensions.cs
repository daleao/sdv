using Netcode;
using StardewValley;
using StardewValley.Locations;

namespace TheLion.Stardew.Professions.Framework.Extensions
{
	public static class MineShaftExtensions
	{
		/// <summary>Whether the current mine level is a safe level; i.e. shouldn't spawn any monsters.</summary>
		/// <param name="shaft">The <see cref="MineShaft"/> instance.</param>
		public static bool IsTreasureOrSafeRoom(this MineShaft shaft)
		{
			return (shaft.mineLevel <= 120 && shaft.mineLevel % 10 == 0) || (shaft.mineLevel == 220 && Game1.player.secretNotesSeen.Contains(10) && !Game1.player.mailReceived.Contains("qiCave")) || ModEntry.ModHelper.Reflection.GetField<NetBool>(shaft, name: "netIsTreasureRoom").GetValue().Value;
		}
	}
}