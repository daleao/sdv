namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Common;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Arsenal.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
[Debug]
internal class MonsterMovePositionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterMovePositionPatcher"/> class.</summary>
    internal MonsterMovePositionPatcher()
    {
        this.Target = this.RequireMethod<Monster>(nameof(Monster.MovePosition));
    }

    #region harmony patches

    /// <summary>Add knockback damage.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MonsterMovePositionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: CollisionDetectedSubroutine(this);
        // After: found_collision = true;
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]),
                    })
                .Move(2)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(MonsterMovePositionPatcher).RequireMethod(nameof(CollisionDetectedSubroutine))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed applying debug transpiler.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void CollisionDetectedSubroutine(Monster monster)
    {
        if (monster.Health <= 0 || !monster.Get_KnockedBack())
        {
            return;
        }

        var velocity = new Vector2(monster.xVelocity, monster.yVelocity);
        var speed = velocity.Length();
        var damage = (int)Math.Pow(speed / 10, 2);
        if (damage <= 0)
        {
            return;
        }

        monster.Health = Math.Max(monster.Health - damage, 0);
        var knockbacker = monster.Get_KnockBacker()!;
        var monsterBox = monster.GetBoundingBox();
        if (monster.Health <= 0)
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

            for (var i = 0; i < knockbacker.enchantments.Count; i++)
            {
                knockbacker.enchantments[i].OnMonsterSlay(monster, location, knockbacker);
            }

            knockbacker.leftRing.Value?.onMonsterSlay(monster, location, knockbacker);
            knockbacker.rightRing.Value?.onMonsterSlay(monster, location, knockbacker);
            if (!location.IsFarm && (monster is not GreenSlime slime || slime.firstGeneration.Value))
            {
                if (knockbacker.IsLocalPlayer)
                {
                    Game1.stats.monsterKilled(monster.Name);
                }
                else if (Game1.IsMasterGame)
                {
                    knockbacker.queueMessage(25, Game1.player, monster.Name);
                }
            }

            location.monsterDrop(monster, monsterBox.Center.X, monsterBox.Center.Y, knockbacker);
            if (!location.IsFarm)
            {
                knockbacker.gainExperience(4, monster.ExperienceGained);
            }

            if (monster.isHardModeMonster.Value)
            {
                Game1.stats.incrementStat("hardModeMonstersKilled", 1);
            }

            location.characters.Remove(monster);
            Game1.stats.MonstersKilled++;
        }
        else
        {
            monster.shedChunks(Game1.random.Next(1, 3));
        }

        monster.currentLocation.debris.Add(new Debris(
            damage,
            new Vector2(monsterBox.Center.X + 16, monsterBox.Center.Y),
            Color.White,
            1f,
            monster));
        monster.Set_KnockedBack(null);
    }

    #endregion injected subroutines
}
