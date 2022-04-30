namespace DaLion.Stardew.Rings.Extensions;

/// <summary>Extensions for the <see cref="int"/> primitive type.</summary>
public static class Int32Extensions
{
    /// <summary>Whether this number is the index of a ring item.</summary>
    public static bool IsRingIndex(this int index)
    {
        return index is >= 516 and <= 534 or 810 or 811;
    }
}