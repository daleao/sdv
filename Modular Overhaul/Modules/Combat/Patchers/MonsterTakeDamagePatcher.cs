namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterTakeDamagePatcher"/> class.</summary>
    internal MonsterTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Monster>(
            nameof(Monster.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string) });
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        if (!base.ApplyImpl(harmony))
        {
            return false;
        }

        foreach (var target in TargetMethods())
        {
            this.Target = target;
            if (!base.ApplyImpl(harmony))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    protected override bool UnapplyImpl(Harmony harmony)
    {
        if (!base.UnapplyImpl(harmony))
        {
            return false;
        }

        foreach (var target in TargetMethods())
        {
            this.Target = target;
            if (!base.UnapplyImpl(harmony))
            {
                return false;
            }
        }

        return true;
    }

    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        return new[]
        {
            typeof(AngryRoger), typeof(Bat), typeof(BigSlime), typeof(BlueSquid), typeof(Bug), typeof(Duggy),
            typeof(DwarvishSentry), typeof(Fly), typeof(Ghost), typeof(GreenSlime), typeof(Grub), typeof(LavaCrab),
            typeof(Mummy), typeof(RockCrab), typeof(RockGolem), typeof(ShadowGirl), typeof(ShadowGuy),
            typeof(ShadowShaman), typeof(SquidKid), typeof(Serpent),
        }.Select(t => t.RequireMethod(
            "takeDamage",
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) }));
    }

    #region harmony patches

    /// <summary>Frozen effect.</summary>
    [HarmonyPrefix]
    private static void MonsterTakeDamagePrefix(Monster __instance, ref int damage)
    {
        if (!__instance.IsFrozen())
        {
            return;
        }

        damage *= 3;
        __instance.Defrost();
    }

    /// <summary>Crits ignore defense, which, btw, actually does something.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MonsterTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: int actualDamage = ...
        // To: int actualDamage = ModEntry.Config.Combat.OverhauledDefense ? TryIgnoreDefense(this, damage) : ...
        try
        {
            var doVanillaDefense = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Monster).RequireField(nameof(Monster.resilience))),
                        new CodeInstruction(OpCodes.Call),
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) })),
                    },
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels)
                .AddLabels(doVanillaDefense)
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
                        new CodeInstruction(OpCodes.Brfalse_S, doVanillaDefense),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(MonsterTakeDamagePatcher).RequireMethod(nameof(TryIgnoreDefense))),
                        new CodeInstruction(OpCodes.Stloc_0),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    },
                    labels)
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_0) })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding crit. ignore for overhauled enemy defense.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int TryIgnoreDefense(Monster monster, int damage)
    {
        return monster.Get_GotCrit()
            ? damage
            : (int)(damage * (10f / (10f + monster.resilience.Value)));
    }

    #endregion injected subroutines
}
