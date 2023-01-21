namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Overhaul.Modules.Arsenal;
using DaLion.Overhaul.Modules.Arsenal.Configs;
using DaLion.Overhaul.Modules.Arsenal.Integrations;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuCore
{
    /// <summary>Register the Arsenal config menu.</summary>
    private void RegisterArsenal()
    {
        this
            .AddPage(OverhaulModule.Arsenal.Namespace, () => "Arsenal Settings")

            .AddSectionTitle(() => "Movement & Control Settings")
            .AddKeyBinding(
                () => "Mod Key",
                () => "The key used for indicating the weapon or slingshot for auto-selection, if enabled.",
                config => config.Tools.ModKey,
                (config, value) => config.Tools.ModKey = value)
            .AddCheckbox(
                () => "Enable Auto-Selection",
                () =>
                    "The best selected weapon or slingshot will be automatically equiped near enemies.",
                config => config.Tools.EnableAutoSelection,
                (config, value) =>
                {
                    config.Tools.EnableAutoSelection = value;
                    if (!value)
                    {
                        ArsenalModule.State.SelectableArsenal = null;
                    }
                })
            .AddColorPicker(
                () => "Selection Border Color",
                () => "The color used to indicate a weapon or slingshot that may be auto-selected.",
                config => config.Arsenal.SelectionBorderColor,
                (config, value) => config.Arsenal.SelectionBorderColor = value,
                Color.Magenta,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)
            .AddCheckbox(
                () => "Face Towards Mouse Cursor",
                () =>
                    "If using mouse and keyboard, turn to face towards the current cursor position before swinging your tools.",
                config => config.Arsenal.FaceMouseCursor,
                (config, value) => config.Arsenal.FaceMouseCursor = value)
            .AddCheckbox(
                () => "Slick Moves",
                () => "Drift in the current running direction when swinging a weapon for more fluid combat movement.",
                config => config.Arsenal.SlickMoves,
                (config, value) => config.Arsenal.SlickMoves = value)

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
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    if (!Context.IsWorldReady)
                    {
                        return;
                    }

                    Utility.iterateAllItems(item =>
                    {
                        if (item is not Ring { ParentSheetIndex: Constants.TopazRingIndex } topaz)
                        {
                            return;
                        }

                        var key = "rings.topaz.description" + (value ? "resist" : "defense");
                        topaz.description = I18n.Get(key);
                    });
                })
            .AddCheckbox(
                () => "Overhauled Knockback",
                () => "Scales down weapon knockback to prevent overshadowing of knockback bonuses, and causes knockbacked enemies to take damage from collisions.",
                config => config.Arsenal.KnockbackDamage,
                (config, value) => config.Arsenal.KnockbackDamage = value)

            .AddSectionTitle(() => "Monster Settings")
            .AddNumberField(
                () => "Monster Health Multiplier",
                () => "Increases the health of all enemies.",
                config => config.Arsenal.MonsterHealthMultiplier,
                (config, value) => config.Arsenal.MonsterHealthMultiplier = value,
                0.25f,
                4f,
                0.25f)
            .AddNumberField(
                () => "Monster Damage Multiplier",
                () => "Increases the damage dealt by all enemies.",
                config => config.Arsenal.MonsterDamageMultiplier,
                (config, value) => config.Arsenal.MonsterDamageMultiplier = value,
                0.25f,
                4f,
                0.25f)
            .AddNumberField(
                () => "Monster Defense Multiplier",
                () => "Increases the damage resistance of all enemies.",
                config => config.Arsenal.MonsterDefenseMultiplier,
                (config, value) => config.Arsenal.MonsterDefenseMultiplier = value,
                0.25f,
                4f,
                0.25f)
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
                () => "Dwarvish Crafting",
                () => "Allows crafting Dwarven and Dragontooth weapons by uncovering ancient Dwarvish blueprints.",
                config => config.Arsenal.DwarvishCrafting,
                (config, value) =>
                {
                    if (value && !ModHelper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
                    {
                        Log.W("Cannot enable Dwarvish Crafting because this feature requires Json Assets which is not installed.");
                        return;
                    }

                    config.Arsenal.DwarvishCrafting = value;
                    if (value && !Globals.DwarvenScrapIndex.HasValue && JsonAssetsIntegration.Instance?.IsRegistered == false)
                    {
                        JsonAssetsIntegration.Instance.Register();
                    }

                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Quests");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
                })
            .AddCheckbox(
                () => "Infinity +1 Weapons",
                () => "Replace lame Galaxy and Infinity weapons with something truly legendary.",
                config => config.Arsenal.InfinityPlusOne,
                (config, value) =>
                {
                    if (value && !ModHelper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
                    {
                        Log.W("Cannot enable Infinity +1 weapons because this feature requires Json Assets which is not installed.");
                        return;
                    }

                    config.Arsenal.InfinityPlusOne = value;
                    if (value && !Globals.HeroSoulIndex.HasValue && JsonAssetsIntegration.Instance?.IsRegistered == false)
                    {
                        JsonAssetsIntegration.Instance.Register();
                    }

                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/WizardHouse");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/StringsFromCSFiles");
                    ModHelper.GameContent.InvalidateCache("TileSheets/Projectiles");
                    if (VanillaTweaksIntegration.Instance?.IsRegistered == true)
                    {
                        ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
                    }

                    if (!Context.IsWorldReady)
                    {
                        return;
                    }

                    if (value)
                    {
                        Arsenal.Utils.RemoveAllIntrinsicEnchantments();
                    }
                    else
                    {
                        Arsenal.Utils.AddAllIntrinsicEnchantments();
                    }
                })

            // page links
            .AddPageLink(OverhaulModule.Arsenal + "/Slingshots", () => "Slingshot Settings", () => "Go to Slingshot settings.")
            .AddPageLink(OverhaulModule.Arsenal + "/Weapons", () => "Weapon Settings", () => "Go to Weapon settings.")

            // slingshot settings
            .AddPage(OverhaulModule.Arsenal + "/Slingshots", () => "Slingshot Settings")
            .AddPageLink(OverhaulModule.Arsenal.Namespace, () => "Back to Arsenal settings")
            .AddCheckbox(
                () => "Enable Crits",
                () => "Allow slingshots to deal critical damage and be affected by critical modifiers.",
                config => config.Arsenal.Slingshots.EnableCrits,
                (config, value) => config.Arsenal.Slingshots.EnableCrits = value)
            .AddCheckbox(
                () => "Enable Enchantments",
                () => "Allow slingshots to be enchanted with weapon enchantments (Prismatic Shard) at the Forge.",
                config => config.Arsenal.Slingshots.EnableEnchants,
                (config, value) => config.Arsenal.Slingshots.EnableEnchants = value)
            .AddCheckbox(
                () => "Enable Forges",
                () => "Allow slingshots to be enchanted with weapon forges (gemstones) at the Forge.",
                config => config.Arsenal.Slingshots.EnableForges,
                (config, value) => config.Arsenal.Slingshots.EnableForges = value)
            .AddCheckbox(
                () => "Enable Special Move",
                () => "Enables a custom stunning smack special move for slingshots.",
                config => config.Arsenal.Slingshots.EnableSpecialMove,
                (config, value) => config.Arsenal.Slingshots.EnableSpecialMove = value)
            .AddCheckbox(
                () => "Remove Grace Period",
                () => "Allows slingshot projectiles to hit targets before the 100ms grace period.",
                config => config.Arsenal.Slingshots.DisableGracePeriod,
                (config, value) => config.Arsenal.Slingshots.DisableGracePeriod = value)
            .AddCheckbox(
                () => "Bullseye Replaces Cursor",
                () => "Draws a bulls-eye instead of the mouse cursor while firing a slingshot. This option does not support pull-back firing for obvious reasons.",
                config => config.Arsenal.Slingshots.BullseyeReplacesCursor,
                (config, value) => config.Arsenal.Slingshots.BullseyeReplacesCursor = value)
            .AddNumberField(
                () => "Auto-Selection Range",
                () => "The minimum distance away from a monster to auto-select your chosen slingshot.",
                config => (int)config.Arsenal.Slingshots.AutoSelectionRange,
                (config, value) => config.Arsenal.Slingshots.AutoSelectionRange = (uint)value,
                2,
                12)

            // weapon settings
            .AddPage(OverhaulModule.Arsenal + "/Weapons", () => "Weapon Settings")
            .AddPageLink(OverhaulModule.Arsenal.Namespace, () => "Back to Arsenal settings")
            .AddCheckbox(
                () => "Enable Combo Hits",
                () => "Replaces vanilla weapon spam with a more strategic combo system.",
                config => config.Arsenal.Weapons.EnableComboHits,
                (config, value) => config.Arsenal.Weapons.EnableComboHits = value)
            .AddCheckbox(
                () => "Enable Hold Swipe",
                () => "Allows performing combos by simply holding the tool button instead of spam-clicking.",
                config => config.Arsenal.Weapons.SwipeHold,
                (config, value) => config.Arsenal.Weapons.SwipeHold = value)
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
                () => "Enable Stabbing Swords",
                () =>
                    "Replace the defensive special move of some swords with an offensive lunge move.\nAFTER DISABLING THIS SETTING YOU MUST TRASH ALL OWNED STABBING SWORDS.",
                config => config.Arsenal.Weapons.EnableStabbySwords,
                (config, value) =>
                {
                    if (ArsenalModule.Config.Weapons.EnableStabbySwords != value && !Context.IsWorldReady)
                    {
                        Log.W("The Stabbing Swords option can only be changed in-game.");
                        return;
                    }

                    config.Arsenal.Weapons.EnableStabbySwords = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
                    if (!Context.IsWorldReady)
                    {
                        return;
                    }

                    if (value)
                    {
                        Utils.ConvertAllStabbingSwords();
                    }
                    else
                    {
                        Utils.RevertAllStabbingSwords();
                    }
                })
            .AddCheckbox(
                () => "Enable Rebalance",
                () => "Rebalances every melee weapon with stats well-suited for this mod's intended experience.",
                config => config.Arsenal.Weapons.EnableRebalance,
                (config, value) =>
                {
                    if (value != config.Arsenal.Weapons.EnableRebalance)
                    {
                        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
                        if (Context.IsWorldReady)
                        {
                            Utils.RefreshAllWeapons(value ? RefreshOption.Randomized : RefreshOption.FromData);
                        }
                    }

                    config.Arsenal.Weapons.EnableRebalance = value;
                })
            .AddCheckbox(
                () => "Enable Retexture",
                () => "Slightly touches up many melee weapons, without changing the vanilla style, to be slightly more realistic or to reflect other changes made by this module.",
                config => config.Arsenal.Weapons.EnableRetexture,
                (config, value) =>
                {
                    config.Arsenal.Weapons.EnableRetexture = value;
                    ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
                })
            .AddCheckbox(
                () => "Enable Enchantments",
                () => "Replaces boring old enchantments with exciting new ones.",
                config => config.Arsenal.Weapons.EnableEnchants,
                (config, value) =>
                {
                    config.Arsenal.Weapons.EnableEnchants = value;
                    ModHelper.GameContent.InvalidateCache("TileSheets/BuffsIcons");
                })
            .AddDropdown(
                () => "Tooltip Style",
                () => "Determines the sprite that appears next to skill bars.",
                config => config.Arsenal.Weapons.WeaponTooltipStyle.ToString(),
                (config, value) => config.Arsenal.Weapons.WeaponTooltipStyle = Enum.Parse<WeaponConfig.TooltipStyle>(value),
                new[] { "Absolute", "Relative" },
                null)
            .AddNumberField(
                () => "Auto-Selection Range",
                () => "The minimum distance away from a monster to auto-select your chosen weapon.",
                config => (int)config.Arsenal.Weapons.AutoSelectionRange,
                (config, value) => config.Arsenal.Weapons.AutoSelectionRange = (uint)value,
                1,
                3);
    }
}
