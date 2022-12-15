namespace DaLion.Overhaul.Modules.Arsenal.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
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
        __instance.MaxHealth = (int)Math.Round(__instance.Health * ArsenalModule.Config.MonsterHealthMultiplier);

        __instance.DamageToFarmer =
            (int)Math.Round(__instance.DamageToFarmer * ArsenalModule.Config.MonsterDamageMultiplier);
        __instance.resilience.Value =
            (int)Math.Round(__instance.resilience.Value * ArsenalModule.Config.MonsterDefenseMultiplier);

        if (ArsenalModule.Config.VariedEncounters)
        {
            __instance.RandomizeStats();
        }

        __instance.Health = __instance.MaxHealth;
    }

    #endregion harmony patches
}
