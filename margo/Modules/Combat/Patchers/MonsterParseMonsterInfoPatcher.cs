namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterParseMonsterInfoPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterParseMonsterInfoPatcher"/> class.</summary>
    internal MonsterParseMonsterInfoPatcher()
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
        if (CombatModule.Config.Enemies.VariedEncounters)
        {
            __instance.RandomizeStats();
        }

        __instance.MaxHealth =
            (int)Math.Round(Math.Max(__instance.MaxHealth + CombatModule.Config.Enemies.MonsterHealthSummand, 1) *
                            CombatModule.Config.Enemies.MonsterHealthMultiplier);
        __instance.DamageToFarmer =
            (int)Math.Round(Math.Max(__instance.DamageToFarmer + CombatModule.Config.Enemies.MonsterDamageSummand, 1) *
                            CombatModule.Config.Enemies.MonsterDamageMultiplier);
        __instance.resilience.Value =
            (int)Math.Round(
                Math.Max(
                    __instance.resilience.Value + (CombatModule.Config.NewResistanceFormula ? 1 : 0) +
                    CombatModule.Config.Enemies.MonsterDefenseSummand,
                    0) * CombatModule.Config.Enemies.MonsterDefenseMultiplier);

        __instance.Health = __instance.MaxHealth;
    }

    #endregion harmony patches
}
