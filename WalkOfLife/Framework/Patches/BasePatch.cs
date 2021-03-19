using Harmony;
using StardewModdingAPI;

namespace TheLion.AwesomeProfessions
{
	internal abstract class BasePatch
	{
		private protected static ProfessionsConfig _config;
		private protected static ProfessionsData _data;
		private protected static IMonitor _monitor;

		internal BasePatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal abstract void Apply(HarmonyInstance harmony);

		/// <summary>Initialize static fields.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="data">The mod persisted data.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		public static void Init(ProfessionsConfig config, ProfessionsData data, IMonitor monitor)
		{
			_config = config;
			_data = data;
			_monitor = monitor;
		}
	}
}
