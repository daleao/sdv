namespace TheLion.AwesomeProfessions
{
	public static partial class Utility
	{
		private static ProfessionsConfig _config;
		private static ProfessionsData _data;

		public static void Init(ProfessionsConfig config)
		{
			_config = config;
		}

		public static void SetData(ProfessionsData data)
		{
			_data = data;
		}
	}
}
