namespace DaLion.Stardew.Slingshots.Extensions;

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
public static class ItemExtensions
{
    /// <summary>Whether the ammo should make squishy noises upon collision.</summary>
    public static bool IsSquishyAmmo(this Item ammo) =>
        ammo.Category is SObject.EggCategory or SObject.FruitsCategory or SObject.VegetableCategory;
}