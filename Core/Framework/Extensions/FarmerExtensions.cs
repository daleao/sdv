namespace DaLion.Core.Framework.Extensions;

#region using directives

using DaLion.Shared.Constants;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    /// <summary>Checks whether the <paramref name="farmer"/> is afflicted with Burn debuff.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> has buff #12, otherwise <see langword="false"/>.</returns>
    internal static bool IsBurning(this Farmer farmer)
    {
        return farmer.hasBuff(BuffIDs.Burnt);
    }

    /// <summary>Removes the Burn debuff from the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    internal static void Unburn(this Farmer farmer)
    {
        farmer.buffs.Remove(BuffIDs.Burnt);
    }

    /// <summary>Checks whether the <paramref name="farmer"/> is afflicted with Freeze debuff.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> has buff #19, otherwise <see langword="false"/>.</returns>
    internal static bool IsFrozen(this Farmer farmer)
    {
        return farmer.hasBuff(BuffIDs.Frozen);
    }

    /// <summary>Removes the Freeze debuff from the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    internal static void Defrost(this Farmer farmer)
    {
        farmer.buffs.Remove(BuffIDs.Frozen);
    }

    /// <summary>Checks whether the <paramref name="farmer"/> is afflicted with Jinxed debuff.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> has buff #14, otherwise <see langword="false"/>.</returns>
    internal static bool IsJinxed(this Farmer farmer)
    {
        return farmer.hasBuff(BuffIDs.Jinxed);
    }
}
