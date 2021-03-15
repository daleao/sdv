using Harmony;
using StardewModdingAPI;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal abstract class BasePatch
	{
		private protected static IMonitor _monitor;

		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal BasePatch(IMonitor monitor)
		{
			_monitor = monitor;
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal abstract void Apply(HarmonyInstance harmony);
	}
}
