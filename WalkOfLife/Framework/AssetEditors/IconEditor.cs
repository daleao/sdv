using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.IO;

namespace TheLion.AwesomeProfessions
{
	internal class IconEditor : IAssetEditor
	{
		/// <inheritdoc/>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(Path.Combine("LooseSprites", "Cursors"));
		}

		/// <inheritdoc/>
		public void Edit<T>(IAssetData asset)
		{
			if (!asset.AssetNameEquals(Path.Combine("LooseSprites", "Cursors")))
				throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");

			// patch modded profession icons
			var editor = asset.AsImage();
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "agriculturist.png")), targetArea: new Rectangle(80, 624, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "angler.png")), targetArea: new Rectangle(32, 640, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "arborist.png")), targetArea: new Rectangle(32, 656, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "artisan.png")), targetArea: new Rectangle(64, 624, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "blaster.png")), targetArea: new Rectangle(16, 672, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "demolitionist.png")), targetArea: new Rectangle(64, 672, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "ecologist.png")), targetArea: new Rectangle(64, 656, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "gambit.png")), targetArea: new Rectangle(48, 688, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "gemologist.png")), targetArea: new Rectangle(80, 672, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "harvester.png")), targetArea: new Rectangle(80, 624, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "lumberjack.png")), targetArea: new Rectangle(0, 656, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "luremaster.png")), targetArea: new Rectangle(64, 640, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "miner.png")), targetArea: new Rectangle(0, 672, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "producer.png")), targetArea: new Rectangle(48, 624, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "prospector.png")), targetArea: new Rectangle(48, 672, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "rancher.png")), targetArea: new Rectangle(0, 624, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "rascal.png")), targetArea: new Rectangle(16, 688, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "scavenger.png")), targetArea: new Rectangle(80, 656, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "slimecharmer.png")), targetArea: new Rectangle(64, 688, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "tapper.png")), targetArea: new Rectangle(48, 656, 16, 16));
			editor.PatchImage(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "trapper.png")), targetArea: new Rectangle(16, 640, 16, 16));
		}
	}
}