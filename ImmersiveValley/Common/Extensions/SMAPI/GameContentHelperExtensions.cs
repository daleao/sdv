namespace DaLion.Common.Extensions.SMAPI;
#region using directives


#endregion using directives

/// <summary>Extensions for the <see cref="IModHelper"/> interface.</summary>
public static class GameContentHelperExtensions
{
    /// <summary>Invalidates the cache for the current asset in English or the current game language.</summary>
    /// <param name="assetName">The asset name without extension.</param>
    public static bool InvalidateCacheAndLocalized(this IGameContentHelper helper, string assetName)
        => helper.InvalidateCache(assetName)
           | (helper.CurrentLocaleConstant != LocalizedContentManager.LanguageCode.en && helper.InvalidateCache(assetName + "." + helper.CurrentLocale));
}