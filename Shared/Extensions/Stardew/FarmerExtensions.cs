namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using DaLion.Shared.Enums;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Gets the tile immediately in front of the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The <see cref="Vector2"/> coordinates of the tile immediately in front of the <paramref name="farmer"/>.</returns>
    public static Vector2 GetFacingTile(this Farmer farmer)
    {
        return farmer.Tile.GetNextTile(farmer.FacingDirection);
    }

    /// <summary>
    ///     Changes the <paramref name="farmer"/>'s <see cref="Direction"/> in order to face the desired
    ///     <paramref name="tile"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="tile">The tile to face.</param>
    /// <returns>The new <see cref="Direction"/>.</returns>
    public static Direction FaceTowardsTile(this Farmer farmer, Vector2 tile)
    {
        if (!farmer.IsLocalPlayer)
        {
            ThrowHelper.ThrowInvalidOperationException("Can only do this for the local player.");
        }

        var direction = (tile - Game1.player.Tile).ToFacingDirection();
        farmer.faceDirection((int)direction);
        return direction;
    }

    /// <summary>Counts the number of completed Monster Eradication goals.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The number of completed Monster Eradication goals.</returns>
    public static int NumMonsterSlayerQuestsCompleted(this Farmer farmer)
    {
        var count = 0;

        if (farmer.mailReceived.Contains("Gil_Slime Charmer Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Savage Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Skeleton Mask"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Insect Head"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Vampire Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Hard Hat"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Burglar's Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Crabshell Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Arcane Hat"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Knight's Helmet"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Napalm Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Telephone"))
        {
            count++;
        }

        return count;
    }
}
