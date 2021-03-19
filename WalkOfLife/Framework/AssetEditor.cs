using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.IO;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Patches mod assets over vanilla assets.</summary>
	internal class AssetEditor : IAssetEditor
	{
		private IContentHelper _content;

		/// <summary>Construct an instance.</summary>
		/// <param name="content">Interface for loading content assets.</param>
		public AssetEditor(IContentHelper content)
		{
			_content = content;
		}

		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(Path.Combine("LooseSprites", "Cursors"));
		}

		/// <summary>Edit a matched asset.</summary>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
		public void Edit<T>(IAssetData asset)
		{
			var editor = asset.AsImage();

			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "agriculturist.png")), targetArea: new Rectangle(80, 624, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "angler.png")), targetArea: new Rectangle(32, 640, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "arborist.png")), targetArea: new Rectangle(32, 656, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "blaster.png")), targetArea: new Rectangle(16, 672, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "demolitionist.png")), targetArea: new Rectangle(64, 672, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "ecologist.png")), targetArea: new Rectangle(64, 656, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "gambit.png")), targetArea: new Rectangle(48, 688, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "gemologist.png")), targetArea: new Rectangle(80, 672, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "harvester.png")), targetArea: new Rectangle(80, 624, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "lumberjack.png")), targetArea: new Rectangle(0, 656, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "luremaster.png")), targetArea: new Rectangle(64, 640, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "miner.png")), targetArea: new Rectangle(0, 672, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "oenologist.png")), targetArea: new Rectangle(64, 624, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", AwesomeProfessions.Config.UseAltProducerIcon ? "producer2.png" : "producer.png")), targetArea: new Rectangle(48, 624, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "prospector.png")), targetArea: new Rectangle(48, 672, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "rancher.png")), targetArea: new Rectangle(0, 624, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "rascal.png")), targetArea: new Rectangle(16, 688, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "scavenger.png")), targetArea: new Rectangle(80, 656, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "tapper.png")), targetArea: new Rectangle(48, 656, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>(Path.Combine("Assets", "trapper.png")), targetArea: new Rectangle(16, 640, 16, 16));
		}
	}
}
