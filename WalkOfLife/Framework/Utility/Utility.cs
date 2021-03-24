namespace TheLion.AwesomeProfessions
{
	public static partial class Utility
	{
		private static ProfessionsConfig Config { get; set; }
		private static ProfessionsData Data { get; set; }

		public static void Init(ProfessionsConfig config)
		{
			Config = config;
		}

		public static void SetData(ProfessionsData data)
		{
			Data = data;
		}
	}
}
