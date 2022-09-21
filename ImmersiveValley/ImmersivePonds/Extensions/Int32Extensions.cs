namespace DaLion.Stardew.Ponds.Extensions;

/// <summary>Extensions for the <see cref="int"/> primitive type.</summary>
public static class Int32Extensions
{
    /// <summary>Determines whether the object <paramref name="index"/> corresponds to algae or seaweed.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds to any of the three algae types, otherwise <see langword="false"/>.</returns>
    public static bool IsAlgaeIndex(this int index)
    {
        return index is Constants.SeaweedIndex or Constants.GreenAlgaeIndex or Constants.WhiteAlgaeIndex;
    }

    /// <summary>Determines whether the object <paramref name="index"/> corresponds to trash.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds any trash item, otherwise <see langword="false"/>.</returns>
    public static bool IsTrashIndex(this int index)
    {
        return index is > 166 and < 173;
    }

    /// <summary>Determines whether the object <paramref name="index"/> corresponds to a non-radioactive metallic ore.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds to either copper, iron, gold or iridium ore, otherwise <see langword="false"/>.</returns>
    public static bool IsNonRadioactiveOreIndex(this int index)
    {
        return index is 378 or 380 or 384 or 386;
    }

    /// <summary>Determines whether the object <paramref name="index"/> corresponds to a non-radioactive metal ingot.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds to either copper, iron, gold or iridium bar, otherwise <see langword="false"/>.</returns>
    public static bool IsNonRadioactiveIngotIndex(this int index)
    {
        return index is 334 or 335 or 336 or 337;
    }
}
