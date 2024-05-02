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
    public static bool IsGlider(this Monster monster)
    {
        return monster.isGlider.Value || monster is Ghost;
    }

    /// <summary>Determines whether the <paramref name="monster"/> is in a state that allows it to suffer damager.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> is not in an invincible state, otherwise <see langword="false"/>.</returns>
    public static bool CanBeDamaged(this Monster monster)
    {
        return !monster.IsInvisible && !monster.isInvincible()
                && (monster is not Bug bug || !bug.isArmoredBug.Value)
                && (monster is not RockCrab crab || crab.Sprite.currentFrame % 4 != 0 ||
                    Reflector
                        .GetUnboundFieldGetter<RockCrab, NetBool>("shellGone")
                        .Invoke(crab).Value)
                && (monster is not LavaLurk lurk || lurk.currentState.Value != LavaLurk.State.Submerged)
                && monster is not Spiker;
    }

    /// <summary>Determines whether the <paramref name="monster"/> is close enough to see the given player.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="other">The target <see cref="Character"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/>'s distance to the <paramref name="other"/> is less than it's aggro threshold, otherwise <see langword="false"/>.</returns>
    public static bool IsWithinCharacterThreshold(this Monster monster, Character? other = null)
    {
        other ??= Game1.player;
        return monster.DistanceTo(other) <= monster.moveTowardPlayerThreshold.Value;
    }

    /// <summary>Causes the <paramref name="monster"/> to die, triggering item drops and quest completion checks as appropriate.</summary>
    /// <param name="monster">The poor <see cref="Monster"/>.</param>
    /// <param name="killer">The murderous <see cref="Farmer"/>.</param>
    public static void Die(this Monster monster, Farmer? killer = null)
    {
        killer ??= Game1.player;
        monster.deathAnimation();
        var location = monster.currentLocation;
        if (ReferenceEquals(location, Game1.player.currentLocation) && !location.IsFarm)
        {
            Game1.player.checkForQuestComplete(null, 1, 1, null, monster.Name, 4);
            var specialOrders = Game1.player.team.specialOrders;
            if (specialOrders is not null)
            {
                foreach (var order in specialOrders)
                {
                    order.onMonsterSlain?.Invoke(Game1.player, monster);
                }
            }
        }

        foreach (var enchantment in killer.enchantments)
        {
            enchantment.OnMonsterSlay(monster, location, killer);
        }

        killer.leftRing.Value?.onMonsterSlay(monster, location, killer);
        killer.rightRing.Value?.onMonsterSlay(monster, location, killer);
        if (!location.IsFarm && (monster is not GreenSlime slime || slime.firstGeneration.Value))
        {
            if (killer.IsLocalPlayer)
            {
                Game1.stats.monsterKilled(monster.Name);
            }
            else if (Game1.IsMasterGame)
            {
                killer.queueMessage(25, Game1.player, monster.Name);
            }
        }

        var monsterBox = monster.GetBoundingBox();
        location.monsterDrop(monster, monsterBox.Center.X, monsterBox.Center.Y, killer);
        if (!location.IsFarm)
        {
            killer.gainExperience(4, monster.ExperienceGained);
        }

        if (monster.isHardModeMonster.Value)
        {
            Game1.stats.Increment("hardModeMonstersKilled", 1);
        }

        //location.characters.Remove(monster); --> let the game handle removing the character in GameLocation.updateCharacters, otherwise the game will forcefully remove the wrong instance
        Game1.stats.MonstersKilled++;
        Log.D($"{monster.Name} (Max Health: {monster.MaxHealth}) was slain by {killer.Name}.");
    }
}
