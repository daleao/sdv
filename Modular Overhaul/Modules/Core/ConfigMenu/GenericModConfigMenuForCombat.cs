namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Shared.Extensions.SMAPI;
using StardewValley.Objects;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the config menu for CMBT.</summary>
    private void AddCombatOptions()
    {
        this
            .AddPage(OverhaulModule.Combat.Namespace, I18n.Gmcm_Cmbt_Heading)

            .AddCheckbox(
                I18n.Gmcm_Cmbt_Enablestatusconditions_Title,
                I18n.Gmcm_Cmbt_Enablestatusconditions_Desc,
                config => config.Combat.EnableStatusConditions,
                (config, value) => config.Combat.EnableStatusConditions = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Overhauleddefense_Title,
                I18n.Gmcm_Cmbt_Overhauleddefense_Desc,
                config => config.Combat.OverhauledDefense,
                (config, value) =>
                {
                    config.Combat.OverhauledDefense = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    if (!Context.IsWorldReady)
                    {
                        return;
                    }

                    Utility.iterateAllItems(item =>
                    {
                        if (item is not Ring { ParentSheetIndex: ItemIDs.TopazRing } topaz)
                        {
                            return;
                        }

                        var key = "rings.topaz.desc" + (value ? "resist" : "defense");
                        topaz.description = _I18n.Get(key);
                    });
                })
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Knockbackdamage_Title,
                I18n.Gmcm_Cmbt_Knockbackdamage_Desc,
                config => config.Combat.KnockbackDamage,
                (config, value) => config.Combat.KnockbackDamage = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Criticalbackattacks_Title,
                I18n.Gmcm_Cmbt_Criticalbackattacks_Desc,
                config => config.Combat.CriticalBackAttacks,
                (config, value) => config.Combat.CriticalBackAttacks = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Shadowyshooterprojectile_Title,
                I18n.Gmcm_Cmbt_Shadowyshooterprojectile_Desc,
                config => config.Combat.ShadowyShooterProjectile,
                (config, value) => config.Combat.ShadowyShooterProjectile = value)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Monsterhealthmultiplier_Title,
                I18n.Gmcm_Cmbt_Monsterhealthmultiplier_Desc,
                config => config.Combat.MonsterHealthMultiplier,
                (config, value) => config.Combat.MonsterHealthMultiplier = value,
                0.25f,
                4f,
                0.25f)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Monsterdamagemultiplier_Title,
                I18n.Gmcm_Cmbt_Monsterdamagemultiplier_Desc,
                config => config.Combat.MonsterDamageMultiplier,
                (config, value) => config.Combat.MonsterDamageMultiplier = value,
                0.25f,
                4f,
                0.25f)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Monsterdefensemultiplier_Title,
                I18n.Gmcm_Cmbt_Monsterdefensemultiplier_Desc,
                config => config.Combat.MonsterDefenseMultiplier,
                (config, value) => config.Combat.MonsterDefenseMultiplier = value,
                0.25f,
                4f,
                0.25f)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Variedencounters_Title,
                I18n.Gmcm_Cmbt_Variedencounters_Desc,
                config => config.Combat.VariedEncounters,
                (config, value) => config.Combat.VariedEncounters = value);
    }
}
