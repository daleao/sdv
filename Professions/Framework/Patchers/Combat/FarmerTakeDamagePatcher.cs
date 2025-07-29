namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmerTakeDamagePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to make Poacher invulnerable in Ambuscade + make Brute unkillable in Frenzy
    ///     + increment Brute rage counter and Limit Break meter.
    /// </summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (this.IsLocalPlayer && this.get_LimitBreak() is Frenzy {IsActive: true}) health = 1;
        // After: if (health <= 0)
        // Before: GetEffectsOfRingMultiplier(863)
        var frenzy = generator.DeclareLocal(typeof(BruteFrenzy));
        try
        {
            var isNotUndyingButMayHaveDailyRevive = generator.DefineLabel();
            helper
                .PatternMatch(
                    [
                        // find index of health <= 0 (start of revive ring effect)
                        new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.health))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Bgt),
                    ])
                .PatternMatch([new CodeInstruction(OpCodes.Bgt)])
                .GetOperand(out var resumeExecution) // copy branch label to resume normal execution
                .Move()
                .AddLabels(isNotUndyingButMayHaveDailyRevive)
                .Insert(
                    [
                        // check if this is the local player
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                        new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                        // check for frenzy
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ProfessionsMod).RequirePropertyGetter(nameof(State))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ProfessionsState).RequirePropertyGetter(nameof(State.LimitBreak))),
                        new CodeInstruction(OpCodes.Isinst, typeof(BruteFrenzy)),
                        new CodeInstruction(OpCodes.Stloc_S, frenzy),
                        new CodeInstruction(OpCodes.Ldloc, frenzy),
                        new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                        // check if it's active
                        new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ILimitBreak).RequirePropertyGetter(nameof(ILimitBreak.IsActive))),
                        new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                        // set health back to 1
                        new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(
                            OpCodes.Stfld,
                            typeof(Farmer).RequireField(nameof(Farmer.health))),
                        // resume execution (skip revive ring effect)
                        new CodeInstruction(OpCodes.Br, resumeExecution),
                    ]);
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
                .PatternMatch(
                    [new CodeInstruction(OpCodes.Ret)],
                    ILHelper.SearchOption.Last) // find index of final return
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_3),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(IncrementBruteCounters))),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Brute rage counter and Limit Break meter.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static void IncrementBruteCounters(Farmer farmer, Monster? damager, int damage)
    {
        if (!farmer.IsLocalPlayer || !farmer.HasProfession(Profession.Brute) || damager is null)
        {
            return;
        }

        State.BruteRageCounter++;
        if (State.LimitBreak is not BruteFrenzy frenzy)
        {
            return;
        }

        if (frenzy.IsActive)
        {
            State.BruteRageCounter++;
            return;
        }

        frenzy.ChargeValue += damage / 4.0;
    }

    #endregion injected

}
