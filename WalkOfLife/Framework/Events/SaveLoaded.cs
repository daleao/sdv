using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public partial class AwesomeProfessions
	{
		/// <summary>Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			Data = Helper.Data.ReadSaveData<ProfessionsData>("thelion.AwesomeProfessions") ?? new ProfessionsData();
		}
	}
}
