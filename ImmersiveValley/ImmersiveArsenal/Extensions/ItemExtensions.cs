namespace DaLion.Stardew.Arsenal.Extensions;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
public static class ItemExtensions
{
    public static bool IsHeroSoul(this Item item) => ModEntry.DynamicGameAssetsApi!.GetDGAItemId(item) ==
                                                     ModEntry.Manifest.UniqueID + "/Hero Soul";
}