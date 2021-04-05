using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class EcologistInventoryChangedEvent : InventoryChangedEvent
	{
		/// <inheritdoc/>
		public override void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
		{
			if (!e.IsLocalPlayer || !(e.Added.Count() > 0)) return;

			// set Ecologist forage quality and increment foraged items count
			foreach (SObject obj in e.Added.Where(item => item is SObject && (((item as SObject).isForage(Game1.currentLocation) && !Utility.IsForagedMineral(item as SObject)) || Utility.IsWildBerry(item as SObject))))
			{
				obj.Quality = Utility.GetEcologistForageQuality();
				AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/ItemsForaged", amount: 1);
			}
		}
	}
}