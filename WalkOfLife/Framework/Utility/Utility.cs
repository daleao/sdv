namespace TheLion.AwesomeProfessions
{
	/// <summary>Holds common methods and properties.</summary>
	public static partial class Utility
	{
		private static ProfessionsConfig Config { get; set; }
		private static ProfessionsData Data { get; set; }

		/// <summary>Initialize static fields.</summary>
		/// <param name="config">The mod settings.</param>
		public static void Init(ProfessionsConfig config)
		{
			Config = config;
		}

		/// <summary>Set mod data reference for patches.</summary>
		/// <param name="data">The mod persisted data.</param>
		public static void SetData(ProfessionsData data)
		{
			Data = data;
		}
	}
}
