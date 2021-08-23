using StardewModdingAPI;
using StardewValley;
using System;
using System.IO;

namespace TheLion.Stardew.Professions.Framework.AssetEditors
{
	public class SASMailEditor : IAssetEditor
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
				string message = ModEntry.I18n.Get("artisan.mailbody" + i, new { farmName = Game1.getFarm().Name });
				editor.Data[$"{ModEntry.UniqueID}/ArtisanAwardNotice{i}"] = message;
			}
		}
	}
}