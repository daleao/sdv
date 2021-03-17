using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public partial class AwesomeProfessions
	{
		/// <summary>Raised after the game writes data to save file (except the initial save creation).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaved(object sender, SavedEventArgs e)
		{
			Helper.Data.WriteSaveData("thelion.AwesomeProfessions", Data);
		}
	}
}
