using StardewModdingAPI;

namespace FlowerMeads.Content;

internal interface IContentSource
{
	T Load<T>(string path);

	IManifest GetManifest();
}
