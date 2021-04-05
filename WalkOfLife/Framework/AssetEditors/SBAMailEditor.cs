using StardewModdingAPI;
using StardewValley;
using System;
using System.IO;

namespace TheLion.AwesomeProfessions
{
	internal class SBAMailEditor : IAssetEditor
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
			for (int i = 0; i < 5; ++i)
			{
				string message = AwesomeProfessions.I18n.Get("brewer.mailbody" + i.ToString(), new { farmName = Game1.getFarm().Name }) + (i == 5 ? AwesomeProfessions.I18n.Get("brewer.mailclose2") : AwesomeProfessions.I18n.Get("brewer.mailclose1"));
				editor.Data[$"{AwesomeProfessions.UniqueID}/BrewerAwardNotice{i}"] = message;
			}
		}
	}
}