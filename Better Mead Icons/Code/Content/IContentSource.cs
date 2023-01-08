using StardewModdingAPI;

namespace DaLion.Meads.Content;

internal interface IContentSource
{
	T Load<T>(string path);

	IManifest GetManifest();
}
