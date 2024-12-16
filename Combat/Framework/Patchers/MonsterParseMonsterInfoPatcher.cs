namespace DaLion.Combat.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterParseMonsterInfoPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterParseMonsterInfoPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MonsterParseMonsterInfoPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<Monster>("parseMonsterInfo");
        if (!base.ApplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireMethod<Monster>("BuffForAdditionalDifficulty");
        return base.ApplyImpl(harmony);
    }

    /// <inheritdoc />
    protected override bool UnapplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<Monster>("parseMonsterInfo");
        if (!base.UnapplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireMethod<Monster>("BuffForAdditionalDifficulty");
        return base.UnapplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Randomize monster stats + apply difficulty sliders.</summary>
    [HarmonyPostfix]
    private static void MonsterParseMonsterInfoPostfix(Monster __instance)
    {
        if (Config.VariedEncounters)
        {
            __instance.RandomizeStats();
        }

        __instance.MaxHealth =
            (int)Math.Round(Math.Max(__instance.MaxHealth + Config.MonsterHealthSummand, 1) *
                            Config.MonsterHealthMultiplier);
        __instance.DamageToFarmer =
            (int)Math.Round(Math.Max(__instance.DamageToFarmer + Config.MonsterDamageSummand, 1) *
                            Config.MonsterDamageMultiplier);
        __instance.resilience.Value =
            (int)Math.Round(
                Math.Max(
                    __instance.resilience.Value + (Config.GeometricMitigationFormula ? 1 : 0) +
                    Config.MonsterDefenseSummand,
                    0) * Config.MonsterDefenseMultiplier);

        __instance.Health = __instance.MaxHealth;
    }

    #endregion harmony patches
}
