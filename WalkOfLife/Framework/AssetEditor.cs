using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.IO;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Patches mod assets over vanilla assets.	</summary>
	internal class AssetEditor : IAssetEditor
	{
		private IContentHelper _Content { get; }
		private ITranslationHelper _I18n { get; }

		/// <summary>Construct an instance.</summary>
		/// <param name="content">Interface for loading content assets.</param>
		public AssetEditor(IContentHelper content, ITranslationHelper i18n)
		{
			_Content = content;
			_I18n = i18n;
		}

		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(Path.Combine("LooseSprites", "Cursors")) || asset.AssetNameEquals(Path.Combine("Data", "mail"));
		}

		/// <summary>Edit a matched asset.</summary>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
		public void Edit<T>(IAssetData asset)
		{
			if (asset.AssetNameEquals(Path.Combine("LooseSprites", "Cursors")))
			{
				var editor = asset.AsImage();
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "agriculturist.png")), targetArea: new Rectangle(80, 624, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "angler.png")), targetArea: new Rectangle(32, 640, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "arborist.png")), targetArea: new Rectangle(32, 656, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "blaster.png")), targetArea: new Rectangle(16, 672, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "demolitionist.png")), targetArea: new Rectangle(64, 672, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "ecologist.png")), targetArea: new Rectangle(64, 656, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "gambit.png")), targetArea: new Rectangle(48, 688, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "gemologist.png")), targetArea: new Rectangle(80, 672, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "harvester.png")), targetArea: new Rectangle(80, 624, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "lumberjack.png")), targetArea: new Rectangle(0, 656, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "luremaster.png")), targetArea: new Rectangle(64, 640, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "miner.png")), targetArea: new Rectangle(0, 672, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "oenologist.png")), targetArea: new Rectangle(64, 624, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", AwesomeProfessions.Config.UseAltProducerIcon ? "producer2.png" : "producer.png")), targetArea: new Rectangle(48, 624, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "prospector.png")), targetArea: new Rectangle(48, 672, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "rancher.png")), targetArea: new Rectangle(0, 624, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "rascal.png")), targetArea: new Rectangle(16, 688, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "scavenger.png")), targetArea: new Rectangle(80, 656, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "tapper.png")), targetArea: new Rectangle(48, 656, 16, 16));
				editor.PatchImage(_Content.Load<Texture2D>(Path.Combine("Assets", "trapper.png")), targetArea: new Rectangle(16, 640, 16, 16));
			}
			else if (asset.AssetNameEquals(Path.Combine("Data", "mail")))
			{
				var editor = asset.AsDictionary<string, string>();
				
				// add oenologist mail
				{
					string awardLevel = Utility.GetOenologyAwardName();
					string awardBonus = string.Format("{0:p0}", Utility.GetOenologistPriceBonus());
					string message = _I18n.Get("oenologist.mailintro", new { farmName = Game1.getFarm().Name, awardLevel, awardBonus }) + (awardLevel.Equals("Best in Show") ? _I18n.Get("oenologist.mailclose2") : _I18n.Get("oenologist.mailclose1")); 
					editor.Data.Add("OenologistAwardNotice", message);
				}

				// add conservationist mail
				{
					string taxBonus = string.Format("{0:p0}", AwesomeProfessions.Data.ConservationistTaxBonusThisSeason);
					string message = _I18n.Get("conservationist.mail", new { taxBonus, farmName = Game1.getFarm().Name });
					editor.Data.Add("ConservationistTaxNotice", message);
				}
			}
			else throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");
		}
	}
}
