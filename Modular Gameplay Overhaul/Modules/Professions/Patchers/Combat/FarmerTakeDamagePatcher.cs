namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    internal FarmerTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to make Poacher invulnerable in Ambuscade + remove vanilla defense cap + make Brute unkillable in Frenzy
    ///     + increment Brute rage counter and ultimate meter.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: else if (this.IsLocalPlayer && this.get_Ultimate() is Ambush {IsActive: true}) monsterDamageCapable = false;
        try
        {
            var alreadyUndamageableOrNotAmbuscade = generator.DefineLabel();
            var ambush = generator.DeclareLocal(typeof(Ambush));
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_0) })
                .Move()
                .AddLabels(alreadyUndamageableOrNotAmbuscade)
                .Insert(
                    new[]
                    {
                        // check if monsterDamageCapable is already false
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                        // check if this is the local player
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                        new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                        // check for ambush
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.Get_Ultimate))),
                        new CodeInstruction(OpCodes.Isinst, typeof(Ambush)),
                        new CodeInstruction(OpCodes.Stloc_S, ambush),
                        new CodeInstruction(OpCodes.Ldloc_S, ambush),
                        new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                        // check if it's active
                        new CodeInstruction(OpCodes.Ldloc_S, ambush),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Ultimate).RequirePropertyGetter(nameof(Ultimate.IsActive))),
                        new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                        // set monsterDamageCapable = false
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Stloc_0),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Poacher Ambush untargetability.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (this.IsLocalPlayer && this.get_Ultimate() is Frenzy {IsActive: true}) health = 1;
        // After: if (health <= 0)
        // Before: GetEffectsOfRingMultiplier(863)
        var frenzy = generator.DeclareLocal(typeof(Frenzy));
        try
        {
            var isNotUndyingButMayHaveDailyRevive = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        // find index of health <= 0 (start of revive ring effect)
                        new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.health))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Bgt),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Bgt) })
                .GetOperand(out var resumeExecution) // copy branch label to resume normal execution
                .Move()
                .AddLabels(isNotUndyingButMayHaveDailyRevive)
                .Insert(
                    new[]
                    {
                        // check if this is the local player
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                        new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                        // check for frenzy
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.Get_Ultimate))),
                        new CodeInstruction(OpCodes.Isinst, typeof(Frenzy)),
                        new CodeInstruction(OpCodes.Stloc_S, frenzy),
                        new CodeInstruction(OpCodes.Ldloc, frenzy),
                        new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                        // check if it's active
                        new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(IUltimate).RequirePropertyGetter(nameof(IUltimate.IsActive))),
                        new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                        // set health back to 1
                        new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(
                            OpCodes.Stfld,
                            typeof(Farmer).RequireField(nameof(Farmer.health))),
                        // resume execution (skip revive ring effect)
                        new CodeInstruction(OpCodes.Br, resumeExecution),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Brute Frenzy immortality.\nHelper returned {ex}");
            return null;
        }

        // Injected: IncrementBruteCounters(this, damager, damage);
        // At: end of method (before return)
        try
        {
            helper
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ret) },
                    ILHelper.SearchOption.Last) // find index of final return
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_3),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(IncrementBruteCounters))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Brute rage counter and ultimate meter.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void IncrementBruteCounters(Farmer farmer, Monster? damager, int damage)
    {
        if (!farmer.IsLocalPlayer || !farmer.HasProfession(Profession.Brute) || damager is null)
        {
            return;
        }

        var frenzy = farmer.Get_Ultimate() as Frenzy;
        ProfessionsModule.State.BruteRageCounter += frenzy?.IsActive == true ? 2 : 1;
        if (frenzy?.IsActive == false)
        {
            frenzy.ChargeValue += damage / 4.0;
        }
    }

    #endregion injected subroutines

}
