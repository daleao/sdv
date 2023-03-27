namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using Modules.Weapons.Extensions;
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
        this.Target = this.RequireMethod<Monster>("parseMonsterInfo");
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        base.ApplyImpl(harmony);

        this.Target = this.RequireMethod<Monster>("BuffForAdditionalDifficulty");
        base.ApplyImpl(harmony);
    }

    /// <inheritdoc />
    protected override void UnapplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<Monster>("parseMonsterInfo");
        base.UnapplyImpl(harmony);

        this.Target = this.RequireMethod<Monster>("BuffForAdditionalDifficulty");
        base.UnapplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Randomize monster stats + apply difficulty sliders.</summary>
    [HarmonyPostfix]
    private static void MonsterParseMonsterInfoPostfix(Monster __instance)
    {
        __instance.MaxHealth = (int)Math.Round(__instance.Health * CombatModule.Config.MonsterHealthMultiplier);

        __instance.DamageToFarmer =
            (int)Math.Round(__instance.DamageToFarmer * CombatModule.Config.MonsterDamageMultiplier);
        __instance.resilience.Value =
            (int)Math.Round((__instance.resilience.Value + (CombatModule.Config.OverhauledDefense ? 1 : 0)) *
                            CombatModule.Config.MonsterDefenseMultiplier);

        if (CombatModule.Config.VariedEncounters)
        {
            __instance.RandomizeStats();
        }

        __instance.Health = __instance.MaxHealth;
    }

    #endregion harmony patches
}
