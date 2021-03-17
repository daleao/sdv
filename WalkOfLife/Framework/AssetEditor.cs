using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace TheLion.AwesomeProfessions
{
	internal class AssetEditor : IAssetEditor
	{
		private IContentHelper _content;

		/// <summary>Construct an instance.</summary>
		/// <param name="content">Interface for accessing game assets.</param>
		public AssetEditor(IContentHelper content)
		{
			_content = content;
		}

		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals("LooseSprites\\Cursors");
		}

		/// <summary>Edit a matched asset.</summary>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
		public void Edit<T>(IAssetData asset)
		{
			var editor = asset.AsImage();

			editor.PatchImage(_content.Load<Texture2D>("Assets\\agriculturist.png"), targetArea: new Rectangle(80, 624, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\angler.png"), targetArea: new Rectangle(32, 640, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\arborist.png"), targetArea: new Rectangle(32, 656, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\blaster.png"), targetArea: new Rectangle(16, 672, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\demolitionist.png"), targetArea: new Rectangle(64, 672, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\ecologist.png"), targetArea: new Rectangle(64, 656, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\gambit.png"), targetArea: new Rectangle(48, 688, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\gemologist.png"), targetArea: new Rectangle(80, 672, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\harvester.png"), targetArea: new Rectangle(80, 624, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\lumberjack.png"), targetArea: new Rectangle(0, 656, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\luremaster.png"), targetArea: new Rectangle(64, 640, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\miner.png"), targetArea: new Rectangle(0, 672, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\oenologist.png"), targetArea: new Rectangle(64, 624, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\" + (AwesomeProfessions.Config.UseAltProducerIcon ? "producer2.png" : "producer.png")), targetArea: new Rectangle(48, 624, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\prospector.png"), targetArea: new Rectangle(48, 672, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\rancher.png"), targetArea: new Rectangle(0, 624, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\rascal.png"), targetArea: new Rectangle(16, 688, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\scavenger.png"), targetArea: new Rectangle(80, 656, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\tapper.png"), targetArea: new Rectangle(48, 656, 16, 16));
			editor.PatchImage(_content.Load<Texture2D>("Assets\\trapper.png"), targetArea: new Rectangle(16, 640, 16, 16));
		}
	}
}
