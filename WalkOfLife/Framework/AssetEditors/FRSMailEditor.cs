using StardewModdingAPI;
using StardewValley;
using System;
using System.IO;

namespace TheLion.Stardew.Professions.Framework.AssetEditors
{
	public class FRSMailEditor : IAssetEditor
	{
		/// <inheritdoc/>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(Path.Combine("Data", "mail"));
		}

		/// <inheritdoc/>
		public void Edit<T>(IAssetData asset)
		{
			if (!asset.AssetNameEquals(Path.Combine("Data", "mail")))
				throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");

			// patch mail from the Ferngill Revenue Service
			var editor = asset.AsDictionary<string, string>();
			var taxBonus = ModEntry.Data.ReadField<float>("ActiveTaxBonusPercent");
			var key = "conservationist.mail1";
			if (taxBonus >= ModEntry.Config.TaxBonusCeiling) key = "conservationist.mail2";

			string message = ModEntry.I18n.Get(key, new { taxBonus = $"{taxBonus:p0}", farmName = Game1.getFarm().Name });
			editor.Data[$"{ModEntry.UniqueID}/ConservationistTaxNotice"] = message;
		}
	}
}