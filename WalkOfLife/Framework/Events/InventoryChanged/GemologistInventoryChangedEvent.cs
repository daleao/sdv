using StardewModdingAPI.Events;
using System.Linq;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class GemologistInventoryChangedEvent : InventoryChangedEvent
	{
		/// <inheritdoc/>
		public override void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
		{
			if (!e.IsLocalPlayer || !(e.Added.Count() > 0)) return;

			// set Gemologist mineral quality and increment collected minerals count
			foreach (SObject obj in e.Added.Where(item => item is SObject && (Utility.IsForagedMineral(item as SObject) || Utility.IsMineralIndex(item.ParentSheetIndex))))
			{
				obj.Quality = Utility.GetGemologistMineralQuality();
				AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/MineralsCollected", amount: 1);
			}
		}
	}
}