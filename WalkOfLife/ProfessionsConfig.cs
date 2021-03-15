using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod user-defined settings.</summary>
	public class ProfessionsConfig
	{
		public bool UseModdedProfessionIcons { get; set; } = true;
		public KeybindList Modkey { get; set; } = KeybindList.ForSingle(SButton.LeftShift);
	}
}
