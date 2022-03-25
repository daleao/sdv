namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using AssetLoaders;
using Events.GameLoop.DayEnding;
using Extensions;
using Ultimate;

using SObject = StardewValley.Object;

#endregion using directives

internal class GameLocationDamageMonsterPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationDamageMonsterPatch()
    {
        Original = RequireMethod<GameLocation>(nameof(GameLocation.damageMonster),
            new[]
            {
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer)
            });
    }

    #region harmony patches

    /// <summary>
    ///     Patch to move critical chance bonus from Scout to Poacher + patch Brute damage bonus + move critical damage
    ///     bonus from Desperado to Poacher Ambush + perform Poacher steal and Piper buff actions + increment Piper Ultimate meter.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GameLocationDamageMonsterTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (who.professions.Contains(<scout_id>) critChance += critChance * 0.5f;
        /// To: if (who.professions.Contains(<poacher_id>) critChance += critChance * 0.5f;

        try
        {
            helper
                .FindProfessionCheck(Farmer.scout) // find index of scout check
                .Advance()
                .SetOperand((int) Profession.Poacher); // replace with Poacher check
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving modded bonus crit chance from Scout to Poacher.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// From: if (who is not null && who.professions.Contains(<fighter_id>) ... *= 1.1f;
        /// To: if (who is not null && who.professions.Contains(<fighter_id>) ... *= who.professions.Contains(100 + <fighter_id>) ? 1.2f : 1.1f;

        var isNotPrestiged = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck((int) Profession.Fighter,
                    true) // find index of brute check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 1.1f) // brute damage multiplier
                )
                .AddLabels(isNotPrestiged)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 10) // arg 10 = Farmer who
                )
                .InsertProfessionCheck((int) Profession.Fighter + 100, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_R4, 1.2f),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching prestiged Fighter bonus damage.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// From: if (who is not null && who.professions.Contains(<brute_id>)) ... *= 1.15f;
        /// From: if (who is not null && who.IsLocalPlayer && who.professions.Contains(<brute_id>)) ... *= 1f + ModEntry.PlayerState.BruteRageCounter * 0.01f;

        try
        {
            helper
                .FindProfessionCheck((int) Profession.Brute,
                    true) // find index of brute check
                .Retreat(2)
                .GetOperand(out var dontBuffDamage)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffDamage),
                    // check for local player
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 10), // arg 10 = Farmer who
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).PropertyGetter(nameof(Farmer.IsLocalPlayer)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 1.15f) // brute damage multiplier
                )
                .SetOperand(1f)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.PlayerState))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<PlayerState>).PropertyGetter(nameof(PerScreen<PlayerState>
                            .Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PlayerState).PropertyGetter(nameof(PlayerState.BruteRageCounter))),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, Frenzy.PCT_INCREMENT_PER_RAGE_F),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Add)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Brute bonus damage.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// From: if (who is not null && crit && who.professions.Contains(<desperado_id>) ... *= 2f;
        /// To: if (who is not null && who.IsLocalPlayer && crit && ModEntry.PlayerState.RegisteredUltimate is Ambush ambush && ambush.ShouldBuffCritPow()) ... *= 2f;

        var ambush = generator.DeclareLocal(typeof(Ambush));
        try
        {
            helper
                .FindProfessionCheck(Farmer.desperado, true) // find index of desperado check
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .GetOperand(out var dontBuffCritPow)
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldnull)
                )
                .ReplaceWith(
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow)
                )
                .Advance()
                .ReplaceWith(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 10) // was cgt ; arg 10 = Farmer who
                )
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).PropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow)
                )
                .Advance()
                .Remove() // was and
                .Advance()
                .Insert(
                    // check for ambush
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.PlayerState))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<PlayerState>).PropertyGetter(nameof(PerScreen<PlayerState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PlayerState).PropertyGetter(nameof(PlayerState.RegisteredUltimate))),
                    new CodeInstruction(OpCodes.Isinst, typeof(Ambush)),
                    new CodeInstruction(OpCodes.Stloc_S, ambush),
                    new CodeInstruction(OpCodes.Ldloc_S, ambush),
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow),
                    // check for crit. pow. buff
                    new CodeInstruction(OpCodes.Ldloc_S, ambush),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Ambush).MethodNamed(nameof(Ambush.ShouldBuffCritPower))),
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving Desperado bonus crit damage to Poacher after-ult.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// Injected: DamageMonsterSubroutine(damageAmount, isBomb, crit, critMultiplier, monster, who);
        ///	Before: if (monster.Health <= 0)

        var didCrit = helper.Locals[7];
        var damageAmount = helper.Locals[8];
        try
        {
            helper
                .FindFirst( // monster.Health <= 0
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Monster).PropertyGetter(nameof(Monster.Health))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Bgt)
                )
                .StripLabels(out var labels) // backup and remove branch labels
                .InsertWithLabels(
                    labels, // restore backed-up labels
                    // prepare arguments
                    new CodeInstruction(OpCodes.Ldloc_S, damageAmount),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 4), // arg 4 = bool isBomb
                    new CodeInstruction(OpCodes.Ldloc_S, didCrit),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 8), // arg 8 = float critMultiplier
                    new CodeInstruction(OpCodes.Ldloc_2), // local 2 = Monster monster
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 10), // arg 10 = Farmer who
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatch).MethodNamed(nameof(DamageMonsterSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while injecting modded Poacher snatch attempt plus Brute Fury and Poacher Cold Blood gauges.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DamageMonsterSubroutine(int damageAmount, bool isBomb, bool didCrit, float critMultiplier,
        Monster monster, Farmer who)
    {
        if (damageAmount <= 0 || isBomb || !who.IsLocalPlayer) return;

        var r = new Random(Guid.NewGuid().GetHashCode());

        // record last time in combat
        if (who.HasProfession(Profession.Brute))
        {
            ModEntry.PlayerState.SecondsSinceLastCombat = 0;
            
            if (who.CurrentTool is MeleeWeapon weapon &&
                ModEntry.PlayerState.RegisteredUltimate is Frenzy frenzy && monster.Health <= 0)
            {
                if (frenzy.IsActive)
                {
                    // lifesteal
                    //var healed = (int) (monster.MaxHealth * 0.01f);
                    //who.health = Math.Min(who.health + healed, who.maxHealth);
                    //who.currentLocation.debris.Add(new(healed,
                    //    new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));

                    // increment kill count
                    ++ModEntry.PlayerState.BruteKillCounter;
                }
                else
                {
                    // increment ultimate
                    frenzy.ChargeValue += weapon.type.Value == MeleeWeapon.club ? 3 : 2; // more if wielding a club
                }
            }
        }

        // try to steal
        if (who.HasProfession(Profession.Poacher))
        {
            if (who.CurrentTool is MeleeWeapon weapon && didCrit)
            {
                if (!monster.ReadDataAs<bool>("Stolen") &&
                    Game1.random.NextDouble() < 0.15)
                {
                    var drops = monster.objectsToDrop.Select(o => new SObject(o, 1) as Item)
                        .Concat(monster.getExtraDropItems()).ToList();
                    var itemToSteal = drops.ElementAtOrDefault(r.Next(drops.Count))?.getOne();
                    if (itemToSteal is not null && !itemToSteal.Name.Contains("Error") &&
                        who.addItemToInventoryBool(itemToSteal))
                    {
                        monster.WriteData("Stolen", bool.TrueString);

                        // play sound effect
                        SoundBank.Play(SFX.PoacherSteal);

                        // if prestiged, reset cooldown
                        if (who.HasProfession(Profession.Poacher, true))
                        {
                            MeleeWeapon.attackSwordCooldown = 0;
                            MeleeWeapon.daggerCooldown = 0;
                            MeleeWeapon.clubCooldown = 0;
                        }
                    }
                }

                // increment Poacher ultimate meter
                if (ModEntry.PlayerState.RegisteredUltimate is Ambush { IsActive: false } ambush1)
                    ambush1.ChargeValue += critMultiplier;
            }

            if (ModEntry.PlayerState.RegisteredUltimate is Ambush { IsActive: true } ambush2)
                ambush2.Deactivate();
        }

        if (!monster.IsSlime() || monster.Health > 0 || !who.HasProfession(Profession.Piper)) return;

        // add Piper buffs
        if (r.NextDouble() < 0.16667 + who.DailyLuck / 2.0)
        {
            var applied = ModEntry.PlayerState.AppliedPiperBuffs;
            var whatToBuff = r.Next(applied.Length);
            if (whatToBuff is not (3 or 6))
            {
                switch (whatToBuff)
                {
                    case 8:
                        if (applied[8] < ModEntry.Config.PiperBuffCap * 8)
                            applied[8] += 8;
                        break;
                    case 7:
                        if (applied[7] < ModEntry.Config.PiperBuffCap * 10)
                            applied[7] += 10;
                        break;
                    default:
                        if (applied[8] < ModEntry.Config.PiperBuffCap)
                            ++applied[whatToBuff];
                        break;
                }

                var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) Profession.Piper;
                Game1.buffsDisplay.removeOtherBuff(buffId);
                Game1.buffsDisplay.addOtherBuff(new(
                    applied[0], applied[1], applied[2], applied[3], applied[4], applied[5],
                    applied[6], applied[7], applied[8], applied[9],
                    applied[10], applied[11],
                    5,
                    "Piper",
                    ModEntry.ModHelper.Translation.Get("piper.name." + (who.IsMale ? "male" : "female")))
                {
                    which = buffId,
                    sheetIndex = 38,
                    millisecondsDuration = 300000,
                    description = ModEntry.ModHelper.Translation.Get("piper.buffdesc", new
                    {
                        farming = applied[0],
                        fishing = applied[1],
                        foraging = applied[5],
                        mining = applied[2],
                        attack = applied[11],
                        defense = applied[10],
                        luck = applied[4],
                        speed = applied[9],
                        energy = applied[7],
                        magnetic = applied[8]
                    })
                });

                EventManager.Enable(typeof(PiperDayEndingEvent));
            }
        }
        
        // heal if prestiged
        if (who.HasProfession(Profession.Piper, true) && r.NextDouble() < 0.333)
        {
                var healed = (int) (monster.MaxHealth * 0.025f);
                who.health = Math.Min(who.health + healed, who.maxHealth);
                who.currentLocation.debris.Add(new(healed,
                    new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));

                who.Stamina = Math.Min(who.Stamina + who.Stamina * 0.01f, who.MaxStamina);
        }

        // increment ultimate meter
        if (ModEntry.PlayerState.RegisteredUltimate is Pandemonia {IsActive: false} pandemonium)
        {
#pragma warning disable CS8509
            var increment = monster switch
#pragma warning restore CS8509
            {
                GreenSlime slime => 4 * slime.Scale,
                BigSlime => 8,
            };

            pandemonium.ChargeValue += increment + r.Next(-2, 3);
        }
    }

    #endregion injected subroutines
}