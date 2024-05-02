namespace DaLion.Shared.Extensions.SMAPI;

/// <summary>Extensions for the <see cref="ITranslationHelper"/> interface.</summary>
public static class TranslationHelperExtensions
{
    /// <summary>Checks whether a translation exists for the given <paramref name="key"/>.</summary>
    /// <param name="helper">The <see cref="ITranslationHelper"/>.</param>
    /// <param name="key">A <see cref="string"/> key to search for.</param>
    /// <param name="tokens">Optional tokens to substitute into the translation.</param>
    /// <returns><see langword="true"/> if a translation exists, otherwise <see langword="false"/>.</returns>
    public static bool Contains(this ITranslationHelper helper, string key, object? tokens = null)
    {
        return tokens is null ? helper.Get(key).HasValue() : helper.Get(key, tokens).HasValue();
    }

    /// <summary>Gets a translation for the given <paramref name="key"/> and return whether it was found.</summary>
    /// <param name="helper">The <see cref="ITranslationHelper"/>.</param>
    /// <param name="key">A <see cref="string"/> key to search for.</param>
    /// <param name="translation">The <see cref="Translation"/>, if found.</param>
    /// <returns><see langword="true"/> if a translation exists, otherwise <see langword="false"/>.</returns>
    public static bool TryGet(this ITranslationHelper helper, string key, out Translation translation)
    {
        translation = helper.Get(key);
        return translation.HasValue();
    }

    /// <summary>Gets a translation for the given <paramref name="key"/> and return whether it was found.</summary>
    /// <param name="helper">The <see cref="ITranslationHelper"/>.</param>
    /// <param name="key">A <see cref="string"/> key to search for.</param>
    /// <param name="tokens">Optional tokens to substitute into the translation.</param>
    /// <param name="translation">The <see cref="Translation"/>, if found.</param>
    /// <returns><see langword="true"/> if a translation exists, otherwise <see langword="false"/>.</returns>
    public static bool TryGet(this ITranslationHelper helper, string key, object tokens, out Translation translation)
    {
        translation = helper.Get(key, tokens);
        return translation.HasValue();
    }
}
