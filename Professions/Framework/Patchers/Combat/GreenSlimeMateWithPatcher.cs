namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Classes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeMateWithPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeMateWithPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GreenSlimeMateWithPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.mateWith));
    }

    #region harmony patches

    /// <summary>Patch to add special Slime variants.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GreenSlimeMateWithTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Ldloc_0)], ILHelper.SearchOption.Last)
                .PatternMatch([new CodeInstruction(OpCodes.Ldarg_0)])
                .Insert(
                [
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GreenSlimeMateWithPatcher).RequireMethod(nameof(DoSlimeGenetics))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed applying knockback damage.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static void DoSlimeGenetics(GreenSlime daddy, GreenSlime mommy, GreenSlime baby)
    {
        var r = new Random(Guid.NewGuid().GetHashCode());

        var inheritHealthFrom = r.Choose(daddy, mommy)!;
        var parentBaseHealth = Data.ReadAs<int>(inheritHealthFrom, DataKeys.BaseHealth);
        var parentHealthIV = Data.ReadAs<int>(inheritHealthFrom, DataKeys.HealthIV);
        var babyHealthIV = (int)Math.Min(r.NextGaussian(parentHealthIV + 1, 2), 10);
        baby.Health = (int)(parentBaseHealth * (1f + (babyHealthIV / 10f)));
        baby.MaxHealth = baby.Health;
        Data.Write(baby, DataKeys.BaseHealth, parentBaseHealth.ToString());
        Data.Write(baby, DataKeys.HealthIV, babyHealthIV.ToString());

        var inheritAttackFrom = r.Choose(daddy, mommy)!;
        var parentBaseAttack = Data.ReadAs<int>(inheritAttackFrom, DataKeys.BaseAttack);
        var parentAttackIV = Data.ReadAs<int>(inheritAttackFrom, DataKeys.AttackIV);
        var babyAttackIV = (int)Math.Min(r.NextGaussian(parentAttackIV + 1, 2), 10);
        baby.DamageToFarmer = (int)(parentBaseAttack * (1f + (babyAttackIV / 10f)));
        Data.Write(baby, DataKeys.BaseAttack, parentBaseAttack.ToString());
        Data.Write(baby, DataKeys.AttackIV, babyAttackIV.ToString());

        var inheritDefenseFrom = r.Choose(daddy, mommy)!;
        var parentBaseDefense = Data.ReadAs<int>(inheritDefenseFrom, DataKeys.BaseDefense);
        var parentDefenseIV = Data.ReadAs<int>(inheritDefenseFrom, DataKeys.DefenseIV);
        var babyDefenseIV = (int)Math.Min(r.NextGaussian(parentDefenseIV + 1, 2), 10);
        baby.resilience.Value = parentBaseDefense + (parentDefenseIV / 2);
        Data.Write(baby, DataKeys.BaseDefense, parentBaseDefense.ToString());
        Data.Write(baby, DataKeys.DefenseIV, babyDefenseIV.ToString());

        if (!Game1.game1.DoesAnyPlayerHaveProfession(Profession.Piper))
        {
            return;
        }

        var goldenRange = new ColorRange(
            [245, 255],
            [205, 225],
            [0, 10]);
        if (goldenRange.Contains(baby.color.Value))
        {
            baby.MakeGoldSlime();
            return;
        }

        var whiteRange = new ColorRange(
            [230, 255],
            [230, 255],
            [230, 255]);
        if (whiteRange.Contains(baby.color.Value) && Game1.random.NextBool(0.012))
        {
            baby.prismatic.Value = true;
            baby.Name = "Prismatic Slime";
        }
    }

    #endregion injections
}
