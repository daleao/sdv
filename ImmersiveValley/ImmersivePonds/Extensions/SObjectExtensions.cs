namespace DaLion.Stardew.Ponds.Extensions;

#region using directives

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Whether a given object is algae or seaweed.</summary>
    public static bool IsAlgae(this SObject @object) =>
        @object.ParentSheetIndex is Constants.SEAWEED_INDEX_I or Constants.GREEN_ALGAE_INDEX_I
            or Constants.WHITE_ALGAE_INDEX_I;

    /// <summary>Whether a given object is a non-radioactive metallic ore.</summary>
    public static bool IsNonRadioactiveOre(this SObject @object) =>
        @object.ParentSheetIndex is 378 or 380 or 384 or 386;

    /// <summary>Whether a given object is a non-radioactive metal ingot.</summary>
    public static bool IsNonRadioactiveIngot(this SObject @object) =>
        @object.ParentSheetIndex is 334 or 335 or 336 or 337;
}