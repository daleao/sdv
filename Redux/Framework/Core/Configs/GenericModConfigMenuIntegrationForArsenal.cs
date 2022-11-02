namespace DaLion.Redux.Framework.Core.Configs;

#region using directives

using DaLion.Shared.Extensions.SMAPI;
using StardewValley.Objects;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuIntegration
{
    /// <summary>Register the Arsenal config menu.</summary>
    private void RegisterArsenal()
    {
        this._configMenu
            .AddPage(ReduxModule.Arsenal.Name, () => "Arsenal Settings")

            .AddSectionTitle(() => "Movement Settings")
            .AddCheckbox(
                () => "Face Towards Mouse Cursor",
                () =>
                    "If using mouse and keyboard, turn to face towards the current cursor position before swinging your tools.",
                config => config.Arsenal.FaceMouseCursor,
                (config, value) =>
                {
                    config.Arsenal.FaceMouseCursor = value;
                })
            .AddCheckbox(
                () => "Slick Moves",
                () =>
                    "Allows the farmer to drift when using weapons while running.",
                config => config.Arsenal.SlickMoves,
                (config, value) => config.Arsenal.FaceMouseCursor = value)

            .AddSectionTitle(() => "Forge Settings")
            .AddCheckbox(
                () => "Rebalanced Forges",
                () => "Improves certain underwhelming forges (analogous to changes by Rings module).",
                config => config.Arsenal.RebalancedForges,
                (config, value) => config.Arsenal.RebalancedForges = value)

            .AddSectionTitle(() => "Stat Settings")
            .AddCheckbox(
                () => "Overhauled Defense",
                () => "Replaces the linear damage mitigation formula with en exponential for better scaling. Applies to enemies, but crit strikes ignore enemy defense. Also allows sword parry damage to scale with defense.",
                config => config.Arsenal.OverhauledDefense,
                (config, value) =>
                {
                    config.Arsenal.OverhauledDefense = value;
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    Utility.iterateAllItems(item =>
                    {
                        if (item is not Ring { ParentSheetIndex: Constants.TopazRingIndex } topaz)
                        {
                            return;
                        }

                        var key = "rings.topaz.description" + (value ? "resist" : "defense");
                        topaz.description = ModEntry.i18n.Get(key);
                    });
                })
            .AddCheckbox(
                () => "Overhauled Knockback",
                () => "Scales down weapon knockback to prevent overshadowing of knockback bonuses, and causes knockbacked enemies to take damage from collisions.",
                config => config.Arsenal.OverhauledKnockback,
                (config, value) => config.Arsenal.OverhauledKnockback = value)

            .AddSectionTitle(() => "Monster Settings")
            .AddNumberField(
                () => "Monster Health Multiplier",
                () => "Increases the health of all enemies.",
                config => config.Arsenal.MonsterHealthMultiplier,
                (config, value) => config.Arsenal.MonsterHealthMultiplier = value,
                1f,
                3f)
            .AddNumberField(
                () => "Monster Damage Multiplier",
                () => "Increases the damage dealt by all enemies.",
                config => config.Arsenal.MonsterDamageMultiplier,
                (config, value) => config.Arsenal.MonsterDamageMultiplier = value,
                1f,
                3f)
            .AddNumberField(
                () => "Monster Defense Multiplier",
                () => "Increases the damage resistance of all enemies.",
                config => config.Arsenal.MonsterDefenseMultiplier,
                (config, value) => config.Arsenal.MonsterDefenseMultiplier = value,
                1f,
                3f)
            .AddCheckbox(
                () => "Varied Encounters",
                () => "Randomizes monster stats, subject to daily luck bias, adding variability to monster encounters.",
                config => config.Arsenal.VariedEncounters,
                (config, value) => config.Arsenal.VariedEncounters = value)

            // page links
            .AddPageLink(ReduxModule.Arsenal + "/Slingshots", () => "Slingshot Settings", () => "Go to Slingshot settings.")
            .AddPageLink(ReduxModule.Arsenal + "/Weapons", () => "Weapon Settings", () => "Go to Weapon settings.")

            // slingshot settings
            .AddPage(ReduxModule.Arsenal + "/Slingshots", () => "Slingshot Settings")
            .AddPageLink(ReduxModule.Arsenal.Name, () => "Back to Arsenal settings")
            .AddCheckbox(
                () => "Allow Crits",
                () => "Allows Slingshot to deal critical damage and be affected by critical modifiers.",
                config => config.Arsenal.Slingshots.AllowCrits,
                (config, value) => config.Arsenal.Slingshots.AllowCrits = value)
            .AddCheckbox(
                () => "Allow Enchants",
                () => "Allow Slingshot to be enchanted with weapon enchantments (Prismatic Shard) at the Forge.",
                config => config.Arsenal.Slingshots.AllowEnchants,
                (config, value) => config.Arsenal.Slingshots.AllowEnchants = value)
            .AddCheckbox(
                () => "Allow Forges",
                () => "Allow Slingshot to be enchanted with weapon forges (gemstones) at the Forge.",
                config => config.Arsenal.Slingshots.AllowForges,
                (config, value) => config.Arsenal.Slingshots.AllowForges = value)
            .AddCheckbox(
                () => "Remove Grace Period",
                () => "Projectiles should not be useless for the first 100ms.",
                config => config.Arsenal.Slingshots.DisableGracePeriod,
                (config, value) => config.Arsenal.Slingshots.DisableGracePeriod = value)

            // weapon settings
            .AddPage(ReduxModule.Arsenal + "/Weapons", () => "Weapon Settings")
            .AddPageLink(ReduxModule.Arsenal.Name, () => "Back to Arsenal settings")
            .AddCheckbox(
                () => "Woody Replaces Rusty",
                () => "Replace the starting Rusty Sword with a Wooden Blade.",
                config => config.Arsenal.Weapons.WoodyReplacesRusty,
                (config, value) => config.Arsenal.Weapons.WoodyReplacesRusty = value)
            .AddCheckbox(
                () => "Bring Back Stabby Swords",
                () =>
                    "Replace the defensive special move of some swords with an offensive lunge move.\nAFTER DISABLING THIS SETTING YOU MUST TRASH ALL OWNED STABBING SWORDS.",
                config => config.Arsenal.Weapons.BringBackStabbySwords,
                (config, value) =>
                {
                    if (ModEntry.Config.Arsenal.Weapons.BringBackStabbySwords != value && !Context.IsWorldReady)
                    {
                        Log.W("The Stabby Swords option can only be changed in-game.");
                        return;
                    }

                    config.Arsenal.Weapons.BringBackStabbySwords = value;
                    if (value)
                    {
                        Framework.Arsenal.Weapons.Utils.ConvertStabbySwords();
                    }
                    else
                    {
                        Framework.Arsenal.Weapons.Utils.RevertStabbySwords();
                    }
                })
            .AddCheckbox(
                () => "Combo Hits",
                () => "Replaces vanilla weapon spam with a more strategic combo system.",
                config => config.Arsenal.Weapons.ComboHits,
                (config, value) => config.Arsenal.Weapons.ComboHits = value)
            .AddCheckbox(
                () => "Immersive Club Smash",
                () =>
                    "A club smash AoE will inflict guaranteed critical damage on burrowing enemies, but completely miss flying enemies.",
                config => config.Arsenal.Weapons.ImmersiveClubSmash,
                (config, value) => config.Arsenal.Weapons.ImmersiveClubSmash = value)
            .AddCheckbox(
                () => "Infinity-Plus-One Weapons",
                () => "Replace lame Galaxy and Infinity weapons with something truly legendary.",
                config => config.Arsenal.Weapons.InfinityPlusOneWeapons,
                (config, value) =>
                {
                    config.Arsenal.Weapons.InfinityPlusOneWeapons = value;
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/StringsFromCSFiles");
                })
            .AddCheckbox(
                () => "Overhauled Enchants",
                () => "Replaces boring old enchantments with exciting new ones.",
                config => config.Arsenal.Weapons.OverhauledEnchants,
                (config, value) => config.Arsenal.Weapons.OverhauledEnchants = value);
    }
}
