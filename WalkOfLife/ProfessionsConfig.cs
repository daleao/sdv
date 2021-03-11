using TheLion.AwesomeProfessions.Configs;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod user-defined settings.</summary>
	public class ProfessionsConfig
	{
		public bool UseModdedProfessionIcons { get; set; } = true;

		public OenologistConfig Oenologist { get; set; } = new();
		public EcologistConfig Ecologist { get; set; } = new();
		public ScavengerConfig Scavenger { get; set; } = new();
		public GemologistConfig Gemologist { get; set; } = new();
		public ProspectorConfig Prospector { get; set; } = new();
		public ConservationistConfig Conservationist { get; set; } = new();
	}
}
