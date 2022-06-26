namespace DaLion.Stardew.Ponds.Extensions;

/// <summary>Extensions for the <see cref="int"/> primitive type.</summary>
public static class Int32Extensions
{
    /// <summary>Whether a given object index corresponds to algae or seaweed.</summary>
    public static bool IsAlgae(this int index)
    {
        return index is Constants.SEAWEED_INDEX_I or Constants.GREEN_ALGAE_INDEX_I
            or Constants.WHITE_ALGAE_INDEX_I;
    }

    /// <summary>Whether a given object index corresponds to trash.</summary>
    public static bool IsTrash(this int objectIndex)
    {
        return objectIndex is > 166 and < 173;
    }
}