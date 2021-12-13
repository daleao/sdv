using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.AssetEditors
{
	public class AchivementsEditor : IAssetEditor
	{
		/// <inheritdoc />
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/achievements"));
		}

		/// <inheritdoc />
		public void Edit<T>(IAssetData asset)
		{
			if (!asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/achievements")))
				throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");

			// patch custom prestige achievements
			var data = asset.AsDictionary<int, string>().Data;

			string name =
				ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
				                                   (Game1.player.IsMale ? "male" : "female"));
			var desc = ModEntry.ModHelper.Translation.Get("prestige.achievement.desc");
			var shouldDisplayBeforeEarned = "false";
			var prerequisite = "-1";
			var hatIndex = string.Empty;

			var newEntry = string.Join("^", name, desc, shouldDisplayBeforeEarned, prerequisite, hatIndex);
			data[name.Hash()] = newEntry;
		}
	}
}