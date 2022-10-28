namespace DaLion.Redux.Core.Extensions;

/// <summary>Extensions for the <see cref="int"/> primitive type.</summary>
internal static class Int32Extensions
{
    /// <summary>Determines whether the object <paramref name="index"/> corresponds to algae or seaweed.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds to any of the three algae types, otherwise <see langword="false"/>.</returns>
    internal static bool IsAlgaeIndex(this int index)
    {
        return index is Constants.GreenAlgaeIndex or Constants.WhiteAlgaeIndex or Constants.SeaweedIndex;
    }

    /// <summary>Determines whether the object <paramref name="index"/> corresponds to trash.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds any trash item, otherwise <see langword="false"/>.</returns>
    internal static bool IsTrashIndex(this int index)
    {
        return index is > 166 and < 173;
    }

    /// <summary>Determines whether the object <paramref name="index"/> corresponds to a non-radioactive metallic ore.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds to either copper, iron, gold or iridium ore, otherwise <see langword="false"/>.</returns>
    internal static bool IsOre(this int index)
    {
        return index is SObject.copper or SObject.iron or SObject.gold or SObject.iridium;
    }
}
