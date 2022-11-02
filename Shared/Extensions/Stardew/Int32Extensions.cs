namespace DaLion.Shared.Extensions.Stardew;

/// <summary>Extensions for the <see cref="int"/> primitive type.</summary>
internal static class Int32Extensions
{
    /// <summary>Determines whether the <paramref name="index"/> corresponds to an algae or seaweed.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds to an algae or seaweed, otherwise <see langword="false"/>.</returns>
    internal static bool IsAlgaeIndex(this int index)
    {
        return index is 152 or 153 or 157;
    }

    /// <summary>Determines whether the object <paramref name="index"/> corresponds to a trash item.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds any trash item, otherwise <see langword="false"/>.</returns>
    internal static bool IsTrashIndex(this int index)
    {
        return index is > 166 and < 173;
    }

    /// <summary>Determines whether the object <paramref name="index"/> corresponds to any metallic ore.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds to either copper, iron, gold, iridium or radioactive ore, otherwise <see langword="false"/>.</returns>
    internal static bool IsOre(this int index)
    {
        return index is SObject.copper or SObject.iron or SObject.gold or SObject.iridium or 909;
    }
}
