namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using DaLion.Common.Harmony;
using Events.GameLoop.DayEnding;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Sounds;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationDamageMonsterPatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.damageMonster), new[]
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
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(
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
                .SetOperand(Profession.Poacher.Value); // replace with Poacher check
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving modded bonus crit chance from Scout to Poacher.\nHelper returned {ex}");
            return null;
        }

        /// From: if (who is not null && who.professions.Contains(<fighter_id>) ... *= 1.1f;
        /// To: if (who is not null && who.professions.Contains(<fighter_id>) ... *= who.professions.Contains(100 + <fighter_id>) ? 1.2f : 1.1f;

        var isNotPrestiged = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck(Profession.Fighter.Value,
                    true) // find index of brute check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 1.1f) // brute damage multiplier
                )
                .AddLabels(isNotPrestiged)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10) // arg 10 = Farmer who
                )
                .InsertProfessionCheck(Profession.Fighter.Value + 100, forLocalPlayer: false)
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

            return null;
        }

        /// From: if (who is not null && who.professions.Contains(<brute_id>)) ... *= 1.15f;
        /// From: if (who is not null && who.IsLocalPlayer && who.professions.Contains(<brute_id>)) ... *= 1f + ModEntry.PlayerState.BruteRageCounter * 0.01f;

        try
        {
            helper
                .FindProfessionCheck(Profession.Brute.Value,
                    true) // find index of brute check
                .Retreat(2)
                .GetOperand(out var dontBuffDamage)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffDamage),
                    // check for local player
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 1.15f) // brute damage multiplier
                )
                .SetOperand(1f)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).RequirePropertyGetter(nameof(ModState.BruteRageCounter))),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, Frenzy.PCT_INCREMENT_PER_RAGE_F),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Add)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Brute bonus damage.\nHelper returned {ex}");

            return null;
        }

        /// From: if (who is not null && crit && who.professions.Contains(<desperado_id>) ... *= 2f;
        /// To: if (who is not null && who.IsLocalPlayer && crit && who.get_Ultimate() is Ambush ambush && ambush.ShouldBuffCritPow()) ... *= 2f;

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
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10) // was cgt ; arg 10 = Farmer who
                )
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow)
                )
                .Advance()
                .Remove() // was and
                .Advance()
                .Insert(
                    // check for ambush
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.get_Ultimate))),
                    new CodeInstruction(OpCodes.Isinst, typeof(Ambush)),
                    new CodeInstruction(OpCodes.Stloc_S, ambush),
                    new CodeInstruction(OpCodes.Ldloc_S, ambush),
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow),
                    // check for crit. pow. buff
                    new CodeInstruction(OpCodes.Ldloc_S, ambush),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Ambush).RequirePropertyGetter(nameof(Ambush.IsGrantingCritBuff))),
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving Desperado bonus crit damage to Poacher after-ult.\nHelper returned {ex}");

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
                        typeof(Monster).RequirePropertyGetter(nameof(Monster.Health))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Bgt)
                )
                .StripLabels(out var labels) // backup and remove branch labels
                .InsertWithLabels(
                    labels, // restore backed-up labels
                            // prepare arguments
                    new CodeInstruction(OpCodes.Ldloc_S, damageAmount),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = bool isBomb
                    new CodeInstruction(OpCodes.Ldloc_S, didCrit),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)8), // arg 8 = float critMultiplier
                    new CodeInstruction(OpCodes.Ldloc_2), // local 2 = Monster monster
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatch).RequireMethod(nameof(DamageMonsterSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed while injecting modded Poacher snatch attempt plus Brute Fury and Poacher Cold Blood gauges.\nHelper returned {ex}");

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
        var ultimate = who.get_Ultimate();
        // record last time in combat
        if (who.HasProfession(Profession.Brute))
        {
            ModEntry.State.SecondsOutOfCombat = 0;

            if (who.CurrentTool is MeleeWeapon weapon && ultimate is Frenzy frenzy && monster.Health <= 0)
            {
                if (frenzy.IsActive)
                {
                    // lifesteal
                    //var healed = (int) (monster.MaxHealth * 0.01f);
                    //who.health = Math.Min(who.health + healed, who.maxHealth);
                    //who.currentLocation.debris.Add(new(healed,
                    //    new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));

                    // increment kill count
                    ++ModEntry.State.BruteKillCounter;
                }
                else
                {
                    // increment ultimate
                    frenzy.ChargeValue += weapon.type.Value == MeleeWeapon.club ? 3 : 2; // more if wielding a club
                }
            }
        }

        // try to steal or assassinate
        if (who.HasProfession(Profession.Poacher))
        {
            if (who.CurrentTool is MeleeWeapon && didCrit)
            {
                if (!monster.Read<bool>("Stolen") &&
                    Game1.random.NextDouble() < 0.2)
                {
                    var drops = monster.objectsToDrop.Select(o => new SObject(o, 1) as Item)
                        .Concat(monster.getExtraDropItems()).ToList();
                    var itemToSteal = drops.ElementAtOrDefault(r.Next(drops.Count))?.getOne();
                    if (itemToSteal is not null && !itemToSteal.Name.Contains("Error") &&
                        who.addItemToInventoryBool(itemToSteal))
                    {
                        monster.Write("Stolen", bool.TrueString);

                        // play sound effect
                        SFX.PoacherSteal.Play();

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
                if (ultimate is Ambush { IsActive: false } ambush1)
                    ambush1.ChargeValue += critMultiplier;
            }

            if (ultimate is Ambush ambush2)
            {
                switch (ambush2.IsActive)
                {
                    case true when monster.Health <= 0:
                        ambush2.Reset();
                        break;
                    case true:
                        ambush2.Deactivate();
                        break;
                    case false when monster.Health <= 0 && ambush2.SecondsOutOfAmbush <= 1.5d:
                        ambush2.Activate();
                        break;
                }
            }
        }

        if (!monster.IsSlime() || monster.Health > 0 || !who.HasProfession(Profession.Piper)) return;

        // add Piper buffs
        if (r.NextDouble() < 0.16667 + who.DailyLuck / 2.0)
        {
            var applied = ModEntry.State.AppliedPiperBuffs;
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

                var buffId = (ModEntry.Manifest.UniqueID + Profession.Piper).GetHashCode();
                Game1.buffsDisplay.removeOtherBuff(buffId);
                Game1.buffsDisplay.addOtherBuff(new(
                    applied[0], applied[1], applied[2], applied[3], applied[4], applied[5],
                    applied[6], applied[7], applied[8], applied[9],
                    applied[10], applied[11],
                    3,
                    "Piper",
                    ModEntry.i18n.Get("piper.name" + (who.IsMale ? ".male" : ".female")))
                {
                    which = buffId,
                    sheetIndex = 38,
                    millisecondsDuration = 180000,
                    description = GetPiperBuffDescription(
                        applied[0],
                        applied[1],
                        applied[5],
                        applied[2],
                        applied[11],
                        applied[10],
                        applied[4],
                        applied[9],
                        applied[7],
                        applied[8]
                    )
                });

                ModEntry.EventManager.Enable<PiperDayEndingEvent>();
            }
        }

        // heal if prestiged
        if (who.HasProfession(Profession.Piper, true) && r.NextDouble() < 0.333 + who.DailyLuck / 2.0)
        {
            var healed = (int)(monster.MaxHealth * 0.025f);
            who.health = Math.Min(who.health + healed, who.maxHealth);
            who.currentLocation.debris.Add(new(healed,
                new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));
            Game1.playSound("healSound");

            who.Stamina = Math.Min(who.Stamina + who.Stamina * 0.01f, who.MaxStamina);
        }

        // increment ultimate meter
        if (ultimate is Concerto { IsActive: false } concerto)
        {
            var increment = monster switch
            {
                GreenSlime slime => 4 * slime.Scale,
                BigSlime => 8,
                _ => 0
            };

            concerto.ChargeValue += increment + r.Next(-2, 3);
        }
    }

    #endregion injected subroutines

    #region helper methods

    private static string GetPiperBuffDescription(int farming, int fishing, int foraging, int mining, int attack, int defense, int luck, int speed, int energy, int magnetic)
    {
        var builder = new StringBuilder();
        if (farming > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.480") + "+" + farming);
            else
                builder.AppendLine("+" + farming + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.480"));
        }

        if (fishing > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.483") + "+" + fishing);
            else
                builder.AppendLine("+" + fishing + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.483"));
        }

        if (foraging > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.492") + "+" + foraging);
            else
                builder.AppendLine("+" + foraging + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.492"));
        }

        if (mining > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.486") + "+" + mining);
            else
                builder.AppendLine("+" + mining + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.486"));
        }

        if (attack > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.504") + "+" + attack);
            else
                builder.AppendLine("+" + attack + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.504"));
        }

        if (defense > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.501") + "+" + defense);
            else
                builder.AppendLine("+" + defense + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.501"));
        }

        if (mining > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.486") + "+" + mining);
            else
                builder.AppendLine("+" + mining + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.486"));
        }

        if (luck > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.489") + "+" + luck);
            else
                builder.AppendLine("+" + luck + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.489"));
        }

        if (speed > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.507") + "+" + speed);
            else
                builder.AppendLine("+" + speed + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.507"));
        }

        if (energy > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.495") + "+" + energy);
            else
                builder.AppendLine("+" + energy + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.495"));
        }

        if (magnetic > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.498") + "+" + magnetic);
            else
                builder.AppendLine("+" + magnetic + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.498"));
        }

        return builder.ToString();
    }

    #endregion helper methods
}