using Harmony;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Harmony patch interface.</summary>
	internal interface IPatch
	{
		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		void Apply(HarmonyInstance harmony);
	}
}