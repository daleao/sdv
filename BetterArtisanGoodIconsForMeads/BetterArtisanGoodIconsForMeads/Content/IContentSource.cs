using StardewModdingAPI;

namespace BetterArtisanGoodIconsForMeads.Content;

internal interface IContentSource
{
	T Load<T>(string path);

	IManifest GetManifest();
}
