namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
public static class ItemExtensions
{
    /// <summary>Whether the ammo is a stone or mineral ore.</summary>
    public static bool IsMineralAmmo(this Item ammo) => ammo.ParentSheetIndex.IsMineralAmmoIndex();

    /// <summary>Whether the ammo should make squishy noises upon collision.</summary>
    public static bool IsSquishyAmmo(this Item ammo) =>
        ammo.Category is SObject.EggCategory or SObject.FruitsCategory or SObject.VegetableCategory;
}