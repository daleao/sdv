namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
internal static class MonsterExtensions
{
    /// <summary>
    ///     Determines whether the <paramref name="monster"/> is an instance of <see cref="GreenSlime"/> or
    ///     <see cref="BigSlime"/>.
    /// </summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> is a <see cref="GreenSlime"/> or <see cref="BigSlime"/>, otherwise <see langword="false"/>.</returns>
    internal static bool IsSlime(this Monster monster)
    {
        return monster is GreenSlime or BigSlime;
    }

    /// <summary>Determines whether the <paramref name="monster"/> is an undead being or void spirit.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> is an undead being or void spirit, otherwise <see langword="false"/>.</returns>
    internal static bool IsUndead(this Monster monster)
    {
        return monster is Ghost or Mummy or ShadowBrute or ShadowGirl or ShadowGuy or ShadowShaman or Skeleton
            or Shooter;
    }

    /// <summary>Determines whether the <paramref name="monster"/> is close enough to see the given player.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="player">The target player.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/>'s distance to the <paramref name="player"/> is less than it's aggro threshold, otherwise <see langword="false"/>.</returns>
    internal static bool IsWithinPlayerThreshold(this Monster monster, Farmer? player = null)
    {
        player ??= Game1.player;
        return monster.DistanceTo(player) <= monster.moveTowardPlayerThreshold.Value;
    }

    /// <summary>Causes the <paramref name="monster"/> to die, triggering item drops and quest completion checks as appropriate.</summary>
    /// <param name="monster">The poor <see cref="Monster"/>.</param>
    /// <param name="killer">The murderous <see cref="Farmer"/>.</param>
    internal static void Die(this Monster monster, Farmer killer)
    {
        monster.deathAnimation();
        var location = monster.currentLocation;
        if (location == Game1.player.currentLocation && !location.IsFarm)
        {
            Game1.player.checkForQuestComplete(null, 1, 1, null, monster.Name, 4);
            var specialOrders = Game1.player.team.specialOrders;
            if (specialOrders is not null)
            {
                for (var i = 0; i < specialOrders.Count; i++)
                {
                    specialOrders[i].onMonsterSlain?.Invoke(Game1.player, monster);
                }
            }
        }

        for (var i = 0; i < killer.enchantments.Count; i++)
        {
            killer.enchantments[i].OnMonsterSlay(monster, location, killer);
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
            Game1.stats.incrementStat("hardModeMonstersKilled", 1);
        }

        //location.characters.Remove(monster); --> let the game handle removing the character in GameLocation.updateCharacters, otherwise the game will forcefully remove the wrong instance
        Game1.stats.MonstersKilled++;
        Log.D($"{monster.Name} (Max Health: {monster.MaxHealth}) was slain by {killer.Name}.");
    }
}
