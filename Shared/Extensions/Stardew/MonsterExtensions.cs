namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using DaLion.Shared.Reflection;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
public static class MonsterExtensions
{
    /// <summary>
    ///     Determines whether the <paramref name="monster"/> is an instance of <see cref="GreenSlime"/> or
    ///     <see cref="BigSlime"/>.
    /// </summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> is a <see cref="GreenSlime"/> or <see cref="BigSlime"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsSlime(this Monster monster)
    {
        return monster is GreenSlime or BigSlime;
    }

    /// <summary>Determines whether the <paramref name="slime"/> is a Tiger Slime.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="slime"/>'s name is "Tiger Slime", otherwise <see langword="false"/>.</returns>
    public static bool IsTigerSlime(this GreenSlime slime)
    {
        return slime.Name == "Tiger Slime";
    }

    /// <summary>Determines whether the <paramref name="monster"/> is an undead being or void spirit.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> is an undead being or void spirit, otherwise <see langword="false"/>.</returns>
    public static bool IsUndead(this Monster monster)
    {
        return monster is Ghost or Mummy or ShadowBrute or ShadowGirl or ShadowGuy or ShadowShaman or Skeleton
            or Shooter;
    }

    /// <summary>Determines whether the <paramref name="monster"/> is a flying enemy (i.e., glider).</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has the <c>isGlider</c> flag or is a <see cref="Ghost"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsFloating(this Monster monster)
    {
        return monster.isGlider.Value || monster is Ghost;
    }

    /// <summary>Determines whether the <paramref name="monster"/> is in a state that allows it to suffer damager.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> is not in an invincible state, otherwise <see langword="false"/>.</returns>
    public static bool CanBeDamaged(this Monster monster)
    {
        return !monster.IsInvisible && !monster.isInvincible();
    }

    /// <summary>Determines whether the <paramref name="monster"/> is in a state that allows it to suffer damager.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> is not in an invincible state, otherwise <see langword="false"/>.</returns>
    public static bool IsArmored(this Monster monster)
    {
        return (monster is Bug bug && bug.isArmoredBug.Value)
               || (monster is RockCrab crab && crab.Sprite.currentFrame % 4 == 0 &&
                   !Reflector
                       .GetUnboundFieldGetter<RockCrab, NetBool>("shellGone")
                       .Invoke(crab).Value)
               || monster is Spiker;
    }

    /// <summary>Determines whether the specified <paramref name="monster"/> is within the moving threshold of this <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="other">The target <see cref="Character"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/>'s distance to the <paramref name="other"/> is less than it's aggro threshold, otherwise <see langword="false"/>.</returns>
    public static bool IsCharacterWithinThreshold(this Monster monster, Character? other = null)
    {
        other ??= Game1.player;
        return monster.SquaredTileDistance(other.Tile) <=
               monster.moveTowardPlayerThreshold.Value * monster.moveTowardPlayerThreshold.Value;
    }
}
