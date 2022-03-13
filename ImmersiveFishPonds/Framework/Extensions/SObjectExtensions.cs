namespace DaLion.Stardew.FishPonds.Framework.Extensions;

#region using directives

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Whether a given object is algae or seaweed.</summary>
    internal static bool IsAlgae(this SObject @object)
    {
        return @object.ParentSheetIndex is Constants.SEAWEED_INDEX_I or Constants.GREEN_ALGAE_INDEX_I
            or Constants.WHITE_ALGAE_INDEX_I;
    }
}