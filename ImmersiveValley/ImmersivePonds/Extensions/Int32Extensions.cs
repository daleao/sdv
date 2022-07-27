namespace DaLion.Stardew.Ponds.Extensions;

/// <summary>Extensions for the <see cref="int"/> primitive type.</summary>
public static class Int32Extensions
{
    /// <summary>Whether a given object index corresponds to algae or seaweed.</summary>
    public static bool IsAlgaeIndex(this int index) => index is Constants.SEAWEED_INDEX_I or Constants.GREEN_ALGAE_INDEX_I or Constants.WHITE_ALGAE_INDEX_I;

    /// <summary>Whether a given object index corresponds to trash.</summary>
    public static bool IsTrashIndex(this int index) => index is > 166 and < 173;

    /// <summary>Whether a given object index corresponds to a non-radioactive metallic ore.</summary>
    public static bool IsNonRadioactiveOreIndex(this int index) => index is 378 or 380 or 384 or 386;

    /// <summary>Whether a given object index corresponds to a non-radioactive metal ingot.</summary>
    public static bool IsNonRadioactiveIngotIndex(this int index) => index is 334 or 335 or 336 or 337;
}