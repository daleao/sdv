namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="int"/> primitive type.</summary>
public static class Int32Extensions
{
    /// <summary>Whether a given object index corresponds to algae or seaweed.</summary>
    public static bool IsAlgae(this int objectIndex) => objectIndex is 152 or 153 or 157;

    /// <summary>Whether a given object index corresponds to trash.</summary>
    public static bool IsTrash(this int objectIndex) => objectIndex is > 166 and < 173;

    /// <summary>Whether a given ammo index corresponds to stone or a mineral ore.</summary>
    public static bool IsMineralAmmo(this int ammoIndex) => ammoIndex is SObject.stone or SObject.copper or SObject.iron
        or SObject.gold or SObject.iridium or 909;
}