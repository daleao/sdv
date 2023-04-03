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
        __instance.MaxHealth = (int)Math.Round(__instance.MaxHealth * CombatModule.Config.MonsterHealthMultiplier);
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
