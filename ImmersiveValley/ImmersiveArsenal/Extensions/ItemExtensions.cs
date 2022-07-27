namespace DaLion.Stardew.Arsenal.Extensions;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
public static class ItemExtensions
{
    /// <summary>Check with DGA if the instance is a Hero Soul.</summary>
    public static bool IsHeroSoul(this Item item) => ModEntry.DynamicGameAssetsApi!.GetDGAItemId(item) ==
                                                     ModEntry.Manifest.UniqueID + "/Hero Soul";

    /// <summary>Whether the ammo should make squishy noises upon collision.</summary>
    public static bool IsSquishyAmmo(this Item ammo) =>
        ammo.Category is Object.EggCategory or Object.FruitsCategory or Object.VegetableCategory;
}