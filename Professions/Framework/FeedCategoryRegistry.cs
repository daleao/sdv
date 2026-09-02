namespace DaLion.Professions.Framework;

#region using directives

using System.Diagnostics.CodeAnalysis;

#endregion using directives

internal static class FeedCategoryRegistry
{
    private static readonly Dictionary<string, FeedCategory> _categories = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets the category for grainy crops like Corn and Wheat.</summary>
    internal static FeedCategory Grains { get; } = Register("Grains");

    /// <summary>Gets the category for leafy crops Bok Choy, Cabbage and Kale.</summary>
    internal static FeedCategory LeafyGreens { get; } = Register("LeafyGreens");

    /// <summary>Gets the category for various type of beans.</summary>
    internal static FeedCategory Legumes { get; } = Register("Legumes");

    /// <summary>Gets the category for root crops like Carrot and Radish.</summary>
    internal static FeedCategory Roots { get; } = Register("Roots");

    /// <summary>Gets the category for starchy tubers like Potato and Yam.</summary>
    internal static FeedCategory Tubers { get; } = Register("Tubers");

    /// <summary>Gets the category for gourd crops like Pumpkin and Squash.</summary>
    internal static FeedCategory Gourds { get; } = Register("Gourds");

    /// <summary>Gets the category for most kinds of fruit.</summary>
    internal static FeedCategory Fruits { get; } = Register("Fruits");

    /// <summary>Gets the category for insect proteins, such as bug meat.</summary>
    internal static FeedCategory Insects { get; } = Register("Insects");

    internal static FeedCategory Register(string id)
    {
        if (_categories.TryGetValue(id, out var existing))
        {
            return existing;
        }

        var category = new FeedCategory(id);
        _categories.Add(id, category);
        return category;
    }

    internal static bool TryGet(string id, [NotNullWhen(true)] out FeedCategory? category)
    {
        if (_categories.TryGetValue(id, out var existing))
        {
            category = existing;
            return true;
        }

        category = null;
        return false;
    }

    internal static FeedCategory GetOrRegister(string id)
    {
        return TryGet(id, out var category) ? (FeedCategory)category : Register(id);
    }
}
