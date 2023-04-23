namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Shared.Extensions.SMAPI;
using StardewValley.Objects;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the Combat config menu.</summary>
    private void RegisterCombat()
    {
        this
            .AddPage(OverhaulModule.Combat.Namespace, () => "Combat Settings")

            .AddCheckbox(
                () => "Overhauled Defense",
                () => "Replaces the linear damage mitigation formula with an exponential formula for better scaling.",
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
                        topaz.description = I18n.Get(key);
                    });
                })
            .AddCheckbox(
                () => "Knockback Damage",
                () => "Causes enemies to suffer collision damage when knocked-back into a wall or other obstacle.",
                config => config.Combat.KnockbackDamage,
                (config, value) => config.Combat.KnockbackDamage = value)
            .AddCheckbox(
                () => "Critical Back Attacks",
                () => "Your attacks on enemies facing away from you gain double crit. chance.",
                config => config.Combat.CriticalBackAttacks,
                (config, value) => config.Combat.CriticalBackAttacks = value)
            .AddNumberField(
                () => "Monster Health Multiplier",
                () => "Multiplies the health of all enemies.",
                config => config.Combat.MonsterHealthMultiplier,
                (config, value) => config.Combat.MonsterHealthMultiplier = value,
                0.25f,
                4f,
                0.25f)
            .AddNumberField(
                () => "Monster Damage Multiplier",
                () => "Multiplies the damage dealt by all enemies.",
                config => config.Combat.MonsterDamageMultiplier,
                (config, value) => config.Combat.MonsterDamageMultiplier = value,
                0.25f,
                4f,
                0.25f)
            .AddNumberField(
                () => "Monster Defense Multiplier",
                () => "Multiplies the damage resistance of all enemies.",
                config => config.Combat.MonsterDefenseMultiplier,
                (config, value) => config.Combat.MonsterDefenseMultiplier = value,
                0.25f,
                4f,
                0.25f)
            .AddCheckbox(
                () => "Varied Encounters",
                () => "Randomizes monster stats, subject to Daily Luck bias, adding variability to monster encounters.",
                config => config.Combat.VariedEncounters,
                (config, value) => config.Combat.VariedEncounters = value);
    }
}
