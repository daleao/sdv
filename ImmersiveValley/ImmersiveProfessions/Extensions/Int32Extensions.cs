namespace DaLion.Stardew.Professions.Extensions;

/// <summary>Extensions for the <see cref="int"/> primitive type.</summary>
public static class Int32Extensions
{
    /// <summary>Determines whether the object <paramref name="index"/> corresponds to algae or seaweed.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds to any of the three algae types, otherwise <see langword="false"/>.</returns>
    public static bool IsAlgaeIndex(this int index)
    {
        return index is 152 or 153 or 157;
    }

    /// <summary>Determines whether the object <paramref name="index"/> corresponds to trash.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds any trash item, otherwise <see langword="false"/>.</returns>
    public static bool IsTrashIndex(this int index)
    {
        return index is > 166 and < 173;
    }

    /// <summary>Determines whether the <paramref name="ammo"/> index corresponds to stone or a mineral ore.</summary>
    /// <param name="ammo">The ammo index.</param>
    /// <returns><see langword="true"/> if the <paramref name="ammo"/> corresponds any of the ores or stone, otherwise <see langword="false"/>.</returns>
    public static bool IsMineralAmmoIndex(this int ammo)
    {
        return ammo is SObject.stone or SObject.copper or SObject.iron
            or SObject.gold or SObject.iridium or 909;
    }
}
