using StardewValley;
using StardewValley.Monsters;

namespace TheLion.Stardew.Professions.Framework.Extensions
{
	public static class MonsterExtensions
	{
		/// <summary>Get the distance between the calling monster and any character.</summary>
		/// <param name="npc">The target character.</param>
		public static double DistanceToCharacter(this Monster m, Character npc)
		{
			return (npc.Position - m.Position).LengthSquared();
		}
	}
}