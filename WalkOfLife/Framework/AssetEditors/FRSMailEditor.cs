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
			string taxBonus = string.Format("{0:p0}", AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/ActiveTaxBonus", float.Parse));
			string message = AwesomeProfessions.I18n.Get("conservationist.mail", new { taxBonus, farmName = Game1.getFarm().Name });
			editor.Data[$"{AwesomeProfessions.UniqueID}/ConservationistTaxNotice"] = message;
		}
	}
}