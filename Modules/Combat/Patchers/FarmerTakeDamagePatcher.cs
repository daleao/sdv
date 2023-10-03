namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Extensions;
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
        this.Prefix!.priority = Priority.First;
    }

    #region harmony patches

    /// <summary>Burn effect + reset seconds-out-of-combat.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static void FarmerTakeDamagePrefix(Farmer __instance, ref int damage, Monster? damager)
    {
        if (damager is not null && damager.IsBurning())
        {
            damage /= 2;
        }

        if (__instance.IsLocalPlayer)
        {
            ModEntry.State.SecondsOutOfCombat = 0;
        }
    }

    /// <summary>Overhaul for farmer defense.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: damage = Math.Max(1, damage - effectiveResilience);
        // To: damage = CalculateDamage(who, damage, effectiveResilience);
        //     x2
        try
        {
            helper
                .ForEach(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = int damage
                        new CodeInstruction(OpCodes.Ldloc_3), // loc 3 = int effectiveResilience
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) })),
                    },
                    () =>
                    {
                        helper
                            .SetOpCode(OpCodes.Ldarg_0) // replace const int 1 with Farmer who
                            .Match(new[] { new CodeInstruction(OpCodes.Sub) })
                            .Remove()
                            .SetOperand(typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(CalculateDamage)));
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding overhauled farmer defense (part 2).\nHelper returned {ex}");
            return null;
        }

        // Injected: if (CombatModule.Config.OverhauledDefense)
        //     skip
        //     {
        //         effectiveResilience >= damage * 0.5f)
        //         effectiveResilience -= (int) (effectiveResilience * Game1.random.Next(3) / 10f);
        //     }
        var skipSoftCap = generator.DefineLabel();
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_3), new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldarg_1), new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                    },
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Combat))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.NewResistanceFormula))),
                        new CodeInstruction(OpCodes.Brtrue_S, skipSoftCap),
                    },
                    labels)
                .Match(
                    new[] { new CodeInstruction(OpCodes.Stloc_3) })
                .Move()
                .AddLabels(skipSoftCap);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting skip over vanilla defense cap.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int CalculateDamage(Farmer who, int rawDamage, int vanillaResistance)
    {
        return CombatModule.Config.NewResistanceFormula
            ? (int)(rawDamage * who.GetOverhauledResilience())
            : Math.Max(1, rawDamage - vanillaResistance);
    }

    #endregion injected subroutines
}
