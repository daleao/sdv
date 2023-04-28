namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Overhaul.Modules.Weapons;
using DaLion.Overhaul.Modules.Weapons.Integrations;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the Weapons config menu.</summary>
    private void RegisterWeapons()
    {
        this
            .AddPage(OverhaulModule.Weapons.Namespace, () => "Weapons Settings")

            .AddSectionTitle(() => "Rebalance Settings")
            .AddCheckbox(
                () => "Enable Rebalance",
                () => "Rebalances every melee weapon, in addition to Mine chests, monster and container drops, weapon shops and many other features.",
                config => config.Weapons.EnableRebalance,
                (config, value) =>
                {
                    if (value != config.Weapons.EnableRebalance)
                    {
                        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
                        if (Context.IsWorldReady)
                        {
                            WeaponsModule.RefreshAllWeapons(value ? RefreshOption.Randomized : RefreshOption.FromData);
                        }
                    }

                    config.Weapons.EnableRebalance = value;
                })
            .AddDropdown(
                () => "Tooltip Style",
                () => "Whether to display weapon stats as their absolute values or as relative to other weapons of the same type.",
                config => config.Weapons.WeaponTooltipStyle.ToString(),
                (config, value) => config.Weapons.WeaponTooltipStyle = Enum.Parse<Config.TooltipStyle>(value),
                new[] { "Absolute", "Relative" },
                null)
            .AddCheckbox(
                () => "Color-Coded For Convenience",
                () => "Whether to colorize weapon names according to the weapon's assigned tier.",
                config => config.Weapons.ColorCodedForYourConvenience,
                (config, value) => config.Weapons.ColorCodedForYourConvenience = value)
            .AddCheckbox(
                () => "Defense Improves Parry",
                () => "Parry damage will increase for each point in defense.",
                config => config.Weapons.DefenseImprovesParry,
                (config, value) => config.Weapons.DefenseImprovesParry = value)
            .AddCheckbox(
                () => "Grounded Club Smash",
                () =>
                    "A club smash AoE will inflict guaranteed critical damage on burrowed enemies, but completely miss flying enemies.",
                config => config.Weapons.GroundedClubSmash,
                (config, value) => config.Weapons.GroundedClubSmash = value)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Combo Settings")
            .AddCheckbox(
                () => "Enable Combo Hits",
                () => "Replaces vanilla weapon spam with a more strategic combo system.",
                config => config.Weapons.EnableComboHits,
                (config, value) => config.Weapons.EnableComboHits = value)
            .AddCheckbox(
                () => "Enable Hold-To-Swipe",
                () => "Allows performing combos by simply holding the tool button instead of spam-clicking.",
                config => config.Weapons.SwipeHold,
                (config, value) => config.Weapons.SwipeHold = value)
            .AddNumberField(
                () => "Stabbing Sword Combo Hits",
                () => "The max. number of consecutive hits in a Stabbing Sword combo.",
                config => config.Weapons.ComboHitsPerWeapon[WeaponType.StabbingSword],
                (config, value) => config.Weapons.ComboHitsPerWeapon[WeaponType.StabbingSword] = value,
                0,
                10)
            .AddNumberField(
                () => "Defense Sword Combo Hits",
                () => "The max. number of consecutive hits in a Defense Sword combo.",
                config => config.Weapons.ComboHitsPerWeapon[WeaponType.DefenseSword],
                (config, value) => config.Weapons.ComboHitsPerWeapon[WeaponType.DefenseSword] = value,
                0,
                10)
            .AddNumberField(
                () => "Club Combo Hits",
                () => "The max. number of consecutive hits in a Club combo.",
                config => config.Weapons.ComboHitsPerWeapon[WeaponType.Club],
                (config, value) => config.Weapons.ComboHitsPerWeapon[WeaponType.Club] = value,
                0,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Stabbing Sword Settings")
            .AddCheckbox(
                () => "Enable Stabbing Swords",
                () => "Replaces the defensive special move of some swords with an offensive lunge move.",
                config => config.Weapons.EnableStabbySwords,
                (config, value) =>
                {
                    if (WeaponsModule.Config.EnableStabbySwords != value && !Context.IsWorldReady)
                    {
                        Log.W("The Stabbing Swords option can only be changed in-game.");
                        return;
                    }

                    config.Weapons.EnableStabbySwords = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
                    if (!Context.IsWorldReady)
                    {
                        return;
                    }

                    if (value)
                    {
                        WeaponsModule.ConvertAllStabbingSwords();
                    }
                    else
                    {
                        WeaponsModule.RevertAllStabbingSwords();
                    }
                })
            .AddDropdown(
                () => "Galaxy Sword Type",
                () => "Whether the Galaxy Sword should be a Stabbing or Defense sword.",
                config => config.Weapons.GalaxySwordType.ToString(),
                (config, value) =>
                {
                    config.Weapons.GalaxySwordType = Enum.Parse<WeaponType>(value);
                    switch (config.Weapons.GalaxySwordType)
                    {
                        case WeaponType.StabbingSword:
                            Collections.StabbingSwords.Add(ItemIDs.GalaxySword);
                            break;
                        case WeaponType.DefenseSword:
                            Collections.StabbingSwords.Remove(ItemIDs.GalaxySword);
                            break;
                        default:
                            ThrowHelperExtensions.ThrowUnexpectedEnumValueException(config.Weapons.GalaxySwordType);
                            return;
                    }

                    Utility.iterateAllItems(item =>
                    {
                        if (item is MeleeWeapon { InitialParentTileIndex: ItemIDs.GalaxySword } galaxy)
                        {
                            galaxy.type.Value = (int)config.Weapons.GalaxySwordType;
                        }
                    });
                },
                new[] { "StabbingSword", "DefenseSword" },
                null)
            .AddDropdown(
                () => "Infinity Blade Type",
                () => "Whether the Galaxy Sword should be a Stabbing or Defense sword.",
                config => config.Weapons.InfinityBladeType.ToString(),
                (config, value) =>
                {
                    config.Weapons.InfinityBladeType = Enum.Parse<WeaponType>(value);
                    switch (config.Weapons.InfinityBladeType)
                    {
                        case WeaponType.StabbingSword:
                            Collections.StabbingSwords.Add(ItemIDs.InfinityBlade);
                            break;
                        case WeaponType.DefenseSword:
                            Collections.StabbingSwords.Remove(ItemIDs.InfinityBlade);
                            break;
                        default:
                            ThrowHelperExtensions.ThrowUnexpectedEnumValueException(config.Weapons.InfinityBladeType);
                            return;
                    }

                    Utility.iterateAllItems(item =>
                    {
                        if (item is MeleeWeapon { InitialParentTileIndex: ItemIDs.InfinityBlade } infinity)
                        {
                            infinity.type.Value = (int)config.Weapons.InfinityBladeType;
                        }
                    });
                },
                new[] { "StabbingSword", "DefenseSword" },
                null)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Quest Settings")
            .AddCheckbox(
                () => "Woody Replaces Rusty",
                () => "Replaces the starting Rusty Sword with a Wooden Blade.",
                config => config.Weapons.WoodyReplacesRusty,
                (config, value) => config.Weapons.WoodyReplacesRusty = value)
            .AddCheckbox(
                () => "Dwarvish Legacy",
                () => "Allows crafting Masterwork weapons by uncovering ancient Dwarvish blueprints.",
                config => config.Weapons.DwarvenLegacy,
                (config, value) =>
                {
                    if (value && !ModHelper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
                    {
                        Log.W("Cannot enable Dwarven Legacy because this feature requires Json Assets, which is not installed.");
                        return;
                    }

                    config.Weapons.DwarvenLegacy = value;
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
                () => "Infinity +1",
                () => "Makes legendary Galaxy and Infinity weapons stronger and harder to get.",
                config => config.Weapons.InfinityPlusOne,
                (config, value) =>
                {
                    if (value && !ModHelper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
                    {
                        Log.W("Cannot enable Infinity +1 weapons because this feature requires Json Assets, which is not installed.");
                        return;
                    }

                    config.Weapons.InfinityPlusOne = value;
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
                        WeaponsModule.RemoveAllIntrinsicEnchantments();
                    }
                    else
                    {
                        WeaponsModule.AddAllIntrinsicEnchantments();
                    }
                })
            .AddNumberField(
                () => "Iridium Bars Per Galaxy Weapon",
                () => "The number of Iridium Bars required to receive a Galaxy weapon.",
                config => config.Weapons.IridiumBarsPerGalaxyWeapon,
                (config, value) => config.Weapons.IridiumBarsPerGalaxyWeapon = value,
                0,
                50)
            .AddNumberField(
                () => "Ruined Blade DoT Multiplier",
                () => "Multiplies the Ruined Blade's damage-over-time effect for greater or lower damage (smaller is lower).",
                config => config.Weapons.RuinBladeDotMultiplier,
                (config, value) => config.Weapons.RuinBladeDotMultiplier = value,
                0,
                2,
                0.05f)
            .AddDropdown(
                () => "Virtue Trial Difficulty",
                () => "The general difficulty for completing each of the Virtue Trials. Changing this setting will immediately update your quest progress!!",
                config => config.Weapons.VirtueTrialTrialDifficulty.ToString(),
                (config, value) =>
                {
                    config.Weapons.VirtueTrialTrialDifficulty = Enum.Parse<Config.TrialDifficulty>(value);
                    if (WeaponsModule.State.Quest is { } quest)
                    {
                        Virtue.List.ForEach(virtue => quest.UpdateVirtueProgress(virtue));
                    }
                },
                new[] { "Easy", "Medium", "Hard" },
                null)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Movement & Control Settings")
            .AddCheckbox(
                () => "Face Towards Mouse Cursor",
                () =>
                    "If using mouse and keyboard, turn to face towards the current cursor position before swinging a weapon.",
                config => config.Weapons.FaceMouseCursor,
                (config, value) => config.Weapons.FaceMouseCursor = value)
            .AddCheckbox(
                () => "Slick Moves",
                () => "Drift in the current running direction when swinging a weapon.",
                config => config.Weapons.SlickMoves,
                (config, value) => config.Weapons.SlickMoves = value)
            .AddCheckbox(
                () => "Enable Auto-Selection",
                () => "The chosen weapon will be automatically equipped near enemies.",
                config => config.Weapons.EnableAutoSelection,
                (config, value) =>
                {
                    config.Weapons.EnableAutoSelection = value;
                    if (!value)
                    {
                        WeaponsModule.State.AutoSelectableWeapon = null;
                    }
                })
            .AddKeyBinding(
                () => "Selection Key",
                () => "The key used for indicating the weapon or slingshot for auto-selection, if enabled.",
                config => config.Weapons.SelectionKey,
                (config, value) => config.Weapons.SelectionKey = value)
            .AddColorPicker(
                () => "Selection Border Color",
                () => "The color used to indicate the chosen weapon for auto-selected.",
                config => config.Weapons.SelectionBorderColor,
                (config, value) => config.Weapons.SelectionBorderColor = value,
                Color.Magenta,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)
            .AddNumberField(
                () => "Auto-Selection Range",
                () => "The minimum distance away from a monster to auto-select your chosen weapon.",
                config => (int)config.Weapons.AutoSelectionRange,
                (config, value) => config.Weapons.AutoSelectionRange = (uint)value,
                1,
                3)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Misc. Settings")
            .AddCheckbox(
                () => "Enable Retexture",
                () => "Applies a Vanilla-friendly retexture that better reflects a weapon`s overhauled type or unique status.",
                config => config.Weapons.EnableRetexture,
                (config, value) =>
                {
                    config.Weapons.EnableRetexture = value;
                    ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
                });
    }
}
