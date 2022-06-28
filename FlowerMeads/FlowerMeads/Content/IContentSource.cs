using StardewModdingAPI;

namespace BetterMeadIcons.Content;

internal interface IContentSource
{
	T Load<T>(string path);

	IManifest GetManifest();
}
