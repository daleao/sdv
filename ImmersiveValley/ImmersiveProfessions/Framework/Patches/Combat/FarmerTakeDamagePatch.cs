namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerTakeDamagePatch()
    {
        Target = RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Patch to make Poacher invulnerable in Ambuscade + remove vanilla defense cap + make Brute unkillable in Frenzy + increment Brute rage counter and ultimate meter.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: else if (this.IsLocalPlayer && this.get_Ultimate() is Ambush {IsActive: true}) monsterDamageCapable = false;

        var alreadyUndamageableOrNotAmbuscade = generator.DefineLabel();
        var ambush = generator.DeclareLocal(typeof(Ambush));
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Stloc_0)
                )
                .Advance()
                .AddLabels(alreadyUndamageableOrNotAmbuscade)
                .InsertInstructions(
                    // check if monsterDamageCapable is already false
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // check if this is the local player
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // check for ambush
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.get_Ultimate))),
                    new CodeInstruction(OpCodes.Isinst, typeof(Ambush)),
                    new CodeInstruction(OpCodes.Stloc_S, ambush),
                    new CodeInstruction(OpCodes.Ldloc_S, ambush),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // check if it's active
                    new CodeInstruction(OpCodes.Ldloc_S, ambush),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Ultimate).RequirePropertyGetter(nameof(Ultimate.IsActive))),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // set monsterDamageCapable = false
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_0)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Poacher Ambush untargetability.\nHelper returned {ex}");
            return null;
        }

        /// Injected: if (this.IsLocalPlayer && this.get_Ultimate() is Frenzy {IsActive: true}) health = 1;
        /// After: if (health <= 0)
        /// Before: GetEffectsOfRingMultiplier(863)

        var isNotUndyingButMayHaveDailyRevive = generator.DefineLabel();
        var frenzy = generator.DeclareLocal(typeof(Frenzy));
        try
        {
            helper
                .FindNext( // find index of health <= 0 (start of revive ring effect)
                    new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                    new CodeInstruction(OpCodes.Ldfld,
                        typeof(Farmer).RequireField(nameof(Farmer.health))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Bgt)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Bgt)
                )
                .GetOperand(out var resumeExecution1) // copy branch label to resume normal execution
                .Advance()
                .AddLabels(isNotUndyingButMayHaveDailyRevive)
                .InsertInstructions(
                    // check if this is the local player
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                    // check for frenzy
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.get_Ultimate))),
                    new CodeInstruction(OpCodes.Isinst, typeof(Frenzy)),
                    new CodeInstruction(OpCodes.Stloc_S, frenzy),
                    new CodeInstruction(OpCodes.Ldloc, frenzy),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                    // check if it's active
                    new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(IUltimate).RequirePropertyGetter(nameof(IUltimate.IsActive))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                    // set health back to 1
                    new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Stfld,
                        typeof(Farmer).RequireField(nameof(Farmer.health))),
                    // resume execution (skip revive ring effect)
                    new CodeInstruction(OpCodes.Br, resumeExecution1)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Brute Frenzy immortality.\nHelper returned {ex}");
            return null;
        }

        /// Injected: if (this.IsLocalPlayer && this.HasProfession(<brute_id>) && damager is not null)
        ///     var frenzy = ModEntry.PlayerState.Ultimate as Frenzy;
        ///     ModEntry.PlayerState.SecondsSinceLastCombat = 0;
        ///     ModEntry.PlayerState.BruteRageCounter = Math.Min(ModEntry.PlayerState.BruteRageCounter + (frenzy?.IsActive ? 2 : 1), 100);
        ///     if (!frenzy.IsActive)
        ///         frenzy.ChargeValue += damage / 4.0;
        /// At: end of method (before return)

        var resumeExecution2 = generator.DefineLabel();
        var doesNotHaveFrenzyOrIsNotActive = generator.DefineLabel();
        var add = generator.DefineLabel();
        try
        {
            helper
                .FindLast( // find index of final return
                    new CodeInstruction(OpCodes.Ret)
                )
                .AddLabels(resumeExecution2) // branch here to skip increments
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution2),
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .InsertProfessionCheck(Profession.Brute.Value, forLocalPlayer: false)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution2),
                    // check if damager null
                    new CodeInstruction(OpCodes.Ldarg_3), // arg 3 = Monster damager
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution2),
                    // load the player state
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.State))), // consumed by setter of BruteRageCounter
                    new CodeInstruction(OpCodes.Dup), // consumed by getter of BruteRageCounter
                    new CodeInstruction(OpCodes.Dup), // consumed by setter of LastTimeInCombat 
                                                      // check for frenzy
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.get_Ultimate))),
                    new CodeInstruction(OpCodes.Isinst, typeof(Frenzy)),
                    new CodeInstruction(OpCodes.Stloc_S, frenzy),
                    // record last time in combat
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).RequirePropertySetter(nameof(ModState.SecondsOutOfCombat))),
                    // increment rage counter
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).RequirePropertyGetter(nameof(ModState.BruteRageCounter))),
                    new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                    new CodeInstruction(OpCodes.Brfalse_S, doesNotHaveFrenzyOrIsNotActive),
                    new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                    new CodeInstruction(OpCodes.Callvirt, typeof(IUltimate).RequirePropertyGetter(nameof(IUltimate.IsActive))),
                    new CodeInstruction(OpCodes.Brfalse_S, doesNotHaveFrenzyOrIsNotActive),
                    new CodeInstruction(OpCodes.Ldc_I4_2),
                    new CodeInstruction(OpCodes.Br_S, add)
                )
                .InsertWithLabels(
                    new[] { doesNotHaveFrenzyOrIsNotActive },
                    new CodeInstruction(OpCodes.Ldc_I4_1)
                )
                .InsertWithLabels(
                    new[] { add },
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 100),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Math).RequireMethod(nameof(Math.Min), new[] { typeof(int), typeof(int) })),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).RequirePropertySetter(nameof(ModState.BruteRageCounter))),
                    // check frenzy once again
                    new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution2),
                    new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                    new CodeInstruction(OpCodes.Callvirt, typeof(IUltimate).RequirePropertyGetter(nameof(IUltimate.IsActive))),
                    new CodeInstruction(OpCodes.Brtrue_S, resumeExecution2),
                    // increment ultimate meter
                    new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(IUltimate).RequirePropertyGetter(nameof(IUltimate.ChargeValue))),
                    new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = int damage
                    new CodeInstruction(OpCodes.Conv_R8),
                    new CodeInstruction(OpCodes.Ldc_R8, 4d),
                    new CodeInstruction(OpCodes.Div),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(IUltimate).RequirePropertySetter(nameof(IUltimate.ChargeValue)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while incrementing Brute rage counter and ultimate meter.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}