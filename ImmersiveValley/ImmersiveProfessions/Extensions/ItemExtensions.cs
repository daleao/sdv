namespace DaLion.Stardew.Professions.Extensions;

#region using directives

#endregion using directives

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
public static class ItemExtensions
{
    /// <summary>Determines whether the <paramref name="ammo"/> is a stone or mineral ore.</summary>
    /// <param name="ammo">The ammo <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="ammo"/> is any of the ores or stone, otherwise <see langword="false"/>.</returns>
    public static bool IsMineralAmmo(this Item ammo)
    {
        return ammo.ParentSheetIndex.IsMineralAmmoIndex();
    }

    /// <summary>Determines whether the <paramref name="ammo"/> should make squishy noises upon collision.</summary>
    /// <param name="ammo">The ammo <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="ammo"/> is an egg, fruit or vegetable, otherwise <see langword="false"/>.</returns>
    public static bool IsSquishyAmmo(this Item ammo)
    {
        return ammo.Category is SObject.EggCategory or SObject.FruitsCategory or SObject.VegetableCategory;
    }
}
