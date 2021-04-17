using StardewModdingAPI;
using StardewValley;
using System;
using System.IO;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	internal class FRSMailEditor : IAssetEditor
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
			var taxBonus = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/ActiveTaxBonusPercent", float.Parse);
			var key = taxBonus switch
			{
				>= 0.37f => "conservationist.mail2",
				_ => "conservationist.mail1"
			};
			string message = AwesomeProfessions.I18n.Get(key, new { taxBonus = $"{taxBonus:p0}", farmName = Game1.getFarm().Name });
			editor.Data[$"{AwesomeProfessions.UniqueID}/ConservationistTaxNotice"] = message;
		}
	}
}