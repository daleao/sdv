using System.Collections.Generic;
using StardewModdingAPI;

namespace DaLion.Meads.Content;

internal abstract class TextureDataContentSource : IContentSource
{
	public abstract CustomTextureData TextureData { get; }

	public abstract T Load<T>(string path);

	public abstract IManifest GetManifest();

	public (string, List<string>, object) GetData()
	{
		return new(TextureData.Mead, TextureData.Flowers, Globals.MeadAsArtisanGoodEnum);
	}
}
