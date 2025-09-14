namespace DaLion.Combat.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Combat.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterTakeDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MonsterTakeDamagePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Monster>(
            nameof(Monster.takeDamage),
            [typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string)]);
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
            Log.D($"Patching {target.DeclaringType} class...");
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
            Log.D($"Unpatching {target.DeclaringType} class...");
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
            typeof(DwarvishSentry), typeof(Fly), typeof(Ghost), typeof(GreenSlime), typeof(Grub),
            typeof(Mummy), typeof(RockCrab), typeof(RockGolem), typeof(ShadowGirl), typeof(ShadowGuy),
            typeof(ShadowShaman), typeof(SquidKid), typeof(Serpent),
        }.Select(t => t.RequireMethod(
            "takeDamage",
            [typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer)]));
    }

    #region harmony patches

    /// <summary>Crits ignore defense, which, btw, actually does something.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? MonsterTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: int actualDamage = ...
        // To: int actualDamage = DoDamageMitigation(damage, this) : ...
        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Monster).RequireField(nameof(Monster.resilience))),
                    ],
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels)
                .Remove()
                .AddLabels(labels)
                .Move(2)
                .RemoveUntil([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Math).RequireMethod(nameof(Math.Max), [typeof(int), typeof(int)]))
                ])
                .Insert([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MonsterTakeDamagePatcher).RequireMethod(nameof(DoDamageMitigation))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding crit ignore enemy defense.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static int DoDamageMitigation(int damage, Monster monster)
    {
        return monster.Get_GotCrit() && Config.CritsIgnoreDefense
            ? damage
            : Config.HyperbolicMitigationFormula
                ? (int)(damage * (10f / (10f + monster.resilience.Value)))
                : Math.Max(1, damage - monster.resilience.Value);
    }

    #endregion injected
}
