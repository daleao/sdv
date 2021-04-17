using StardewModdingAPI;
using StardewValley;
using System;
using System.IO;

namespace TheLion.AwesomeProfessions
{
	internal class SASMailEditor : IAssetEditor
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

			// patch mail from the Stardew Winemaker's Association
			var editor = asset.AsDictionary<string, string>();
			for (var i = 0; i < 5; ++i)
			{
				string message = AwesomeProfessions.I18n.Get("artisan.mailbody" + i, new { farmName = Game1.getFarm().Name });
				editor.Data[$"{AwesomeProfessions.UniqueID}/ArtisanAwardNotice{i}"] = message;
			}
		}
	}
}