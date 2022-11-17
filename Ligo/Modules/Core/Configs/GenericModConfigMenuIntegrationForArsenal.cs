namespace DaLion.Ligo.Modules.Core.Configs;

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
            .AddPage(LigoModule.Arsenal.Namespace, () => "Arsenal Settings")

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

            .AddSectionTitle(() => "Misc. Settings")
            .AddCheckbox(
                () => "Woody Replaces Rusty",
                () => "Replace the starting Rusty Sword with a Wooden Blade.",
                config => config.Arsenal.WoodyReplacesRusty,
                (config, value) => config.Arsenal.WoodyReplacesRusty = value)
            .AddCheckbox(
                () => "Infinity +1 Weapons",
                () => "Replace lame Galaxy and Infinity weapons with something truly legendary.",
                config => config.Arsenal.InfinityPlusOne,
                (config, value) =>
                {
                    config.Arsenal.InfinityPlusOne = value;
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/StringsFromCSFiles");
                })

            // page links
            .AddPageLink(LigoModule.Arsenal + "/Slingshots", () => "Slingshot Settings", () => "Go to Slingshot settings.")
            .AddPageLink(LigoModule.Arsenal + "/Weapons", () => "Weapon Settings", () => "Go to Weapon settings.")

            // slingshot settings
            .AddPage(LigoModule.Arsenal + "/Slingshots", () => "Slingshot Settings")
            .AddPageLink(LigoModule.Arsenal.Namespace, () => "Back to Arsenal settings")
            .AddCheckbox(
                () => "Allow Crits",
                () => "Allow slingshots to deal critical damage and be affected by critical modifiers.",
                config => config.Arsenal.Slingshots.AllowCrits,
                (config, value) => config.Arsenal.Slingshots.AllowCrits = value)
            .AddCheckbox(
                () => "Allow Enchants",
                () => "Allow slingshots to be enchanted with weapon enchantments (Prismatic Shard) at the Forge.",
                config => config.Arsenal.Slingshots.AllowEnchants,
                (config, value) => config.Arsenal.Slingshots.AllowEnchants = value)
            .AddCheckbox(
                () => "Allow Forges",
                () => "Allow slingshots to be enchanted with weapon forges (gemstones) at the Forge.",
                config => config.Arsenal.Slingshots.AllowForges,
                (config, value) => config.Arsenal.Slingshots.AllowForges = value)
            .AddCheckbox(
                () => "Allow Special Move",
                () => "Enables a custom stunning smack special move for slingshots.",
                config => config.Arsenal.Slingshots.AllowSpecial,
                (config, value) => config.Arsenal.Slingshots.AllowSpecial = value)
            .AddCheckbox(
                () => "Remove Grace Period",
                () => "Allows slingshot projectiles to hit targets before the 100ms grace period.",
                config => config.Arsenal.Slingshots.DisableGracePeriod,
                (config, value) => config.Arsenal.Slingshots.DisableGracePeriod = value)

            // weapon settings
            .AddPage(LigoModule.Arsenal + "/Weapons", () => "Weapon Settings")
            .AddPageLink(LigoModule.Arsenal.Namespace, () => "Back to Arsenal settings")
            .AddCheckbox(
                () => "Allow Combo Hits",
                () => "Replaces vanilla weapon spam with a more strategic combo system.",
                config => config.Arsenal.Weapons.AllowComboHits,
                (config, value) => config.Arsenal.Weapons.AllowComboHits = value)
            .AddCheckbox(
                () => "Grounded Club Smash",
                () =>
                    "A club smash AoE will inflict guaranteed critical damage on burrowed enemies, but completely miss flying enemies.",
                config => config.Arsenal.Weapons.GroundedClubSmash,
                (config, value) => config.Arsenal.Weapons.GroundedClubSmash = value)
            .AddCheckbox(
                () => "Defense Improves Parry",
                () => "Parry damage will increase by 10% for each point in defense.",
                config => config.Arsenal.Weapons.DefenseImprovesParry,
                (config, value) => config.Arsenal.Weapons.DefenseImprovesParry = value)
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
                        Arsenal.Weapons.Utils.ConvertAllStabbySwords();
                    }
                    else
                    {
                        Arsenal.Weapons.Utils.RevertAllStabbySwords();
                    }
                })
            .AddCheckbox(
                () => "Rebalance Weapons",
                () => "Rebalances every melee weapon with stats well-suited for this mod's intended experience.",
                config => config.Arsenal.Weapons.RebalanceWeapons,
                (config, value) =>
                {
                    config.Arsenal.Weapons.RebalanceWeapons = value;
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/weapons");
                    Arsenal.Weapons.Utils.UpdateAllWeapons();
                })
            .AddCheckbox(
                () => "Use Ligo Enchantments",
                () => "Replaces boring old enchantments with exciting new ones.",
                config => config.Arsenal.Weapons.LigoEnchants,
                (config, value) => config.Arsenal.Weapons.LigoEnchants = value);
    }
}
