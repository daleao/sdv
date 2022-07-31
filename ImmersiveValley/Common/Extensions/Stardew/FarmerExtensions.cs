namespace DaLion.Common.Extensions.Stardew;

#region using directives

using Enums;
using Microsoft.Xna.Framework;
using ModData;
using System;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Change the farmer's facing direction to face the desired tile.</summary>
    /// <returns>A vector representation of the new facing direction.</returns>
    public static void FaceTowardsTile(this Farmer farmer, Vector2 tile)
    {
        if (!farmer.IsLocalPlayer) ThrowHelper.ThrowInvalidOperationException("Can only do this for the local player.");

        var (x, y) = tile - Game1.player.getTileLocation();
        FacingDirection direction;
        if (Math.Abs(x) >= Math.Abs(y))
            direction = x < 0 ? FacingDirection.Left : FacingDirection.Right;
        else
            direction = y > 0 ? FacingDirection.Down : FacingDirection.Up;

        farmer.faceDirection((int)direction);
    }

    /// <inheritdoc cref="ModDataIO.Read"/>
    public static string Read(this Farmer farmer, string field, string defaultValue = "", string modId = "") =>
        ModDataIO.Read(farmer, field, defaultValue, modId);

    /// <inheritdoc cref="ModDataIO.Read{T}"/>
    public static T Read<T>(this Farmer farmer, string field, T defaultValue = default, string modId = "") where T : struct =>
        ModDataIO.Read(farmer, field, defaultValue, modId);

    /// <inheritdoc cref="ModDataIO.Write"/>
    public static void Write(this Farmer farmer, string field, string? value) =>
        ModDataIO.Write(farmer, field, value);

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists"/>
    public static void WriteIfNotExists(this Farmer farmer, string field, string? value) =>
        ModDataIO.WriteIfNotExists(farmer, field, value);

    /// <inheritdoc cref="ModDataIO.Append"/>
    public static void Append(this Farmer farmer, string field, string value, string separator = ",") =>
        ModDataIO.Append(farmer, field, value, separator);

    /// <inheritdoc cref="ModDataIO.Increment{T}"/>
    public static void Increment<T>(this Farmer farmer, string field, T amount) where T : struct =>
        ModDataIO.Increment(farmer, field, amount);

    /// <summary>Increment the value of a numeric field in the farmer's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment(this Farmer farmer, string field) =>
        ModDataIO.Increment(farmer, field, 1);
}