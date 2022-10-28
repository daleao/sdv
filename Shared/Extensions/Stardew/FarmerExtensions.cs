namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.ModData;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>
    ///     Changes the <paramref name="farmer"/>'s <see cref="FacingDirection"/> in order to face the desired
    ///     <paramref name="tile"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="tile">The tile to face.</param>
    public static void FaceTowardsTile(this Farmer farmer, Vector2 tile)
    {
        if (!farmer.IsLocalPlayer)
        {
            ThrowHelper.ThrowInvalidOperationException("Can only do this for the local player.");
        }

        var (x, y) = tile - Game1.player.getTileLocation();
        FacingDirection direction;
        if (Math.Abs(x) >= Math.Abs(y))
        {
            direction = x < 0 ? FacingDirection.Left : FacingDirection.Right;
        }
        else
        {
            direction = y > 0 ? FacingDirection.Down : FacingDirection.Up;
        }

        farmer.faceDirection((int)direction);
    }

    /// <inheritdoc cref="ModDataIO.Read(Farmer, string, string, string)"/>
    public static string Read(this Farmer farmer, string field, string defaultValue = "", string modId = "")
    {
        return ModDataIO.Read(farmer, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Read{T}(Farmer, string, T, string)"/>
    public static T Read<T>(this Farmer farmer, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return ModDataIO.Read(farmer, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Write(Farmer, string, string?)"/>
    public static void Write(this Farmer farmer, string field, string? value)
    {
        ModDataIO.Write(farmer, field, value);
    }

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists(Farmer, string, string?)"/>
    public static void WriteIfNotExists(this Farmer farmer, string field, string? value)
    {
        ModDataIO.WriteIfNotExists(farmer, field, value);
    }

    /// <inheritdoc cref="ModDataIO.Append(Farmer, string, string, string)"/>
    public static void Append(this Farmer farmer, string field, string value, string separator = ",")
    {
        ModDataIO.Append(farmer, field, value, separator);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(Farmer, string, T)"/>
    public static void Increment<T>(this Farmer farmer, string field, T amount)
        where T : struct
    {
        ModDataIO.Increment(farmer, field, amount);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(Farmer, string, T)"/>
    public static void Increment(this Farmer farmer, string field)
    {
        ModDataIO.Increment(farmer, field, 1);
    }
}
