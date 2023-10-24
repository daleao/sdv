namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Overhaul.Modules.Combat;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GenericModConfigMenu;
using Microsoft.Xna.Framework;
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

            // general
            .AddSectionTitle(I18n.Gmcm_Cmbt_General_Heading)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_EnableStatusConditions_Title,
                I18n.Gmcm_Cmbt_EnableStatusConditions_Desc,
                config => config.Combat.EnableStatusConditions,
                (config, value) => config.Combat.EnableStatusConditions = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_OverhauledDefense_Title,
                I18n.Gmcm_Cmbt_OverhauledDefense_Desc,
                config => config.Combat.NewResistanceFormula,
                (config, value) =>
                {
                    config.Combat.NewResistanceFormula = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    if (!Context.IsWorldReady)
                    {
                        return;
                    }

                    Utility.iterateAllItems(item =>
                    {
                        if (item is not Ring { ParentSheetIndex: ObjectIds.TopazRing } topaz)
                        {
                            return;
                        }

                        var key = "rings.topaz.desc" + (value ? "resist" : "defense");
                        topaz.description = _I18n.Get(key);
                    });
                })
            .AddCheckbox(
                I18n.Gmcm_Cmbt_KnockbackDamage_Title,
                I18n.Gmcm_Cmbt_KnockbackDamage_Desc,
                config => config.Combat.EnableKnockbackDamage,
                (config, value) => config.Combat.EnableKnockbackDamage = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_CriticalBackAttacks_Title,
                I18n.Gmcm_Cmbt_CriticalBackAttacks_Desc,
                config => config.Combat.CriticalBackAttacks,
                (config, value) => config.Combat.CriticalBackAttacks = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_EnableOverhaul_Title,
                I18n.Gmcm_Cmbt_EnableOverhaul_Desc,
                config => config.Combat.EnableWeaponOverhaul,
                (config, value) =>
                {
                    if (value != config.Combat.EnableWeaponOverhaul)
                    {
                        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
                        if (Context.IsWorldReady)
                        {
                            CombatModule.RefreshAllWeapons(value
                                ? WeaponRefreshOption.Randomized
                                : WeaponRefreshOption.FromData);
                        }
                    }

                    config.Combat.EnableWeaponOverhaul = value;
                })
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Enchantments_RebalancedGemstones_Title,
                I18n.Gmcm_Cmbt_Enchantments_RebalancedGemstones_Desc,
                config => config.Combat.RebalancedGemstones,
                (config, value) => config.Combat.RebalancedGemstones = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Enchantments_New_Title,
                I18n.Gmcm_Cmbt_Enchantments_New_Desc,
                config => config.Combat.NewPrismaticEnchantments,
                (config, value) => config.Combat.NewPrismaticEnchantments = value)
            .AddHorizontalRule()

            // page links
            .AddMultiPageLinkOption(
                getOptionName: I18n.Gmcm_Cmbt_Items_Title,
                pages: new[] { "Melee", "Ranged", "Rings", "Quests", "Enemies", "ControlsAndUi" },
                getPageId: page => OverhaulModule.Combat.Namespace + $"/{page}",
                getPageName: page =>
                    page == "ControlsAndUi"
                        ? I18n.Gmcm_Headings_ControlsAndUi()
                        : _I18n.Get("gmcm.cmbt." + page.ToLowerInvariant() + ".heading"),
                getColumnsFromWidth: _ => 2)

            #region melee

            .AddPage(OverhaulModule.Combat.Namespace + "/Melee", I18n.Gmcm_Cmbt_Melee_Heading)
            .AddPageLink(OverhaulModule.Combat.Namespace, I18n.Gmcm_Cmbt_Back)
            .AddVerticalSpace()
            .AddSectionTitle(I18n.Gmcm_Cmbt_Melee_Combo_Heading)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Melee_Combo_EnableComboHits_Title,
                I18n.Gmcm_Cmbt_Melee_Combo_EnableComboHits_Desc,
                config => config.Combat.EnableComboHits,
                (config, value) => config.Combat.EnableComboHits = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Melee_Combo_SwipeHold_Title,
                I18n.Gmcm_Cmbt_Melee_Combo_SwipeHold_Desc,
                config => config.Combat.SwipeHold,
                (config, value) => config.Combat.SwipeHold = value)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Melee_Combo_StabbySwordHits_Title,
                I18n.Gmcm_Cmbt_Melee_Combo_StabbySwordHits_Desc,
                config => config.Combat.ComboHitsPerWeapon[WeaponType.StabbingSword],
                (config, value) => config.Combat.ComboHitsPerWeapon[WeaponType.StabbingSword] = value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Melee_Combo_DefenseSwordHits_Title,
                I18n.Gmcm_Cmbt_Melee_Combo_DefenseSwordHits_Desc,
                config => config.Combat.ComboHitsPerWeapon[WeaponType.DefenseSword],
                (config, value) => config.Combat.ComboHitsPerWeapon[WeaponType.DefenseSword] = value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Melee_Combo_ClubHits_Title,
                I18n.Gmcm_Cmbt_Melee_Combo_ClubHits_Desc,
                config => config.Combat.ComboHitsPerWeapon[WeaponType.Club],
                (config, value) => config.Combat.ComboHitsPerWeapon[WeaponType.Club] = value,
                0,
                10)

            .AddSectionTitle(I18n.Gmcm_Cmbt_Melee_Sword_Heading)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Melee_Sword_EnableStabby_Title,
                I18n.Gmcm_Cmbt_Melee_Sword_EnableStabby_Desc,
                config => config.Combat.EnableStabbingSwords,
                (config, value) =>
                {
                    if (CombatModule.Config.EnableStabbingSwords != value && !Context.IsWorldReady)
                    {
                        Log.W("The Stabbing Swords option can only be changed in-game.");
                        return;
                    }

                    config.Combat.EnableStabbingSwords = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
                    if (!Context.IsWorldReady)
                    {
                        return;
                    }

                    if (value)
                    {
                        CombatModule.ConvertAllStabbingSwords();
                    }
                    else
                    {
                        CombatModule.RevertAllStabbingSwords();
                    }
                })
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Melee_Sword_DefenseImprovesParry_Title,
                I18n.Gmcm_Cmbt_Melee_Sword_DefenseImprovesParry_Desc,
                config => config.Combat.DefenseImprovesParry,
                (config, value) => config.Combat.DefenseImprovesParry = value)

            .AddSectionTitle(I18n.Gmcm_Cmbt_Melee_Club_Heading)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Melee_Club_GroundedClubSmash_Title,
                I18n.Gmcm_Cmbt_Melee_Club_GroundedClubSmash_Desc,
                config => config.Combat.GroundedClubSmash,
                (config, value) => config.Combat.GroundedClubSmash = value)

            #endregion melee

            #region ranged

            .AddPage(OverhaulModule.Combat.Namespace + "/Ranged", I18n.Gmcm_Cmbt_Ranged_Heading)
            .AddPageLink(OverhaulModule.Combat.Namespace, I18n.Gmcm_Cmbt_Back)
            .AddVerticalSpace()
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Ranged_EnableCriticalHits_Title,
                I18n.Gmcm_Cmbt_Ranged_EnableCriticalHits_Desc,
                config => config.Combat.EnableRangedCriticalHits,
                (config, value) => config.Combat.EnableRangedCriticalHits = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Ranged_EnableSpecialMove_Title,
                I18n.Gmcm_Cmbt_Ranged_EnableSpecialMove_Desc,
                config => config.Combat.EnableSlingshotSpecialMove,
                (config, value) => config.Combat.EnableSlingshotSpecialMove = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Ranged_RemoveGracePeriod_Title,
                I18n.Gmcm_Cmbt_Ranged_RemoveGracePeriod_Desc,
                config => config.Combat.RemoveSlingshotGracePeriod,
                (config, value) => config.Combat.RemoveSlingshotGracePeriod = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Ranged_EnableInfinitySlingshot_Title,
                I18n.Gmcm_Cmbt_Ranged_EnableInfinitySlingshot_Desc,
                config => config.Combat.EnableInfinitySlingshot,
                (config, value) => config.Combat.EnableInfinitySlingshot = value)

            #endregion ranged

            #region rings

            .AddPage(OverhaulModule.Combat.Namespace + "/Rings", I18n.Gmcm_Cmbt_Rings_Heading)
            .AddPageLink(OverhaulModule.Combat.Namespace, I18n.Gmcm_Cmbt_Back)
            .AddVerticalSpace()
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Rings_RebalanceRings_Title,
                I18n.Gmcm_Cmbt_Rings_RebalanceRings_Desc,
                config => config.Combat.RebalancedRings,
                (config, value) =>
                {
                    config.Combat.RebalancedRings = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                })
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Rings_CraftableGemRings_Title,
                I18n.Gmcm_Cmbt_Rings_CraftableGemRings_Desc,
                config => config.Combat.CraftableGemstoneRings,
                (config, value) =>
                {
                    config.Combat.CraftableGemstoneRings = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
                })
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Cmbt_Rings_Infinity_Heading)
            .SetTitleScreenOnlyForNextOptions(true)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Rings_Infinity_Enable_Title,
                I18n.Gmcm_Cmbt_Rings_Infinity_Enable_Desc,
                config => config.Combat.EnableInfinityBand,
                (config, value) =>
                {
                    if (value && JsonAssetsIntegration.Instance?.IsLoaded != true)
                    {
                        Log.W(
                            "Cannot enable The One Iridium Band because this feature requires Json Assets which is not installed.");
                        return;
                    }

                    config.Combat.EnableInfinityBand = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
                    if (value && (JsonAssetsIntegration.Instance?.IsRegistered != true ||
                                  (Context.IsWorldReady && !JsonAssetsIntegration.InfinityBandIndex.HasValue)))
                    {
                        Log.E(
                            "Cannot enable The One Infinity Band because the Json Assets integration was not registered.");
                    }
                })
            .AddParagraph(I18n.Gmcm_Cmbt_Rings_Infinity_Disclaimer)
            .SetTitleScreenOnlyForNextOptions(false)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Rings_Resonance_Enable_Title,
                I18n.Gmcm_Cmbt_Rings_Resonance_Enable_Desc,
                config => config.Combat.EnableResonances,
                (config, value) => config.Combat.EnableResonances = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Rings_Resonance_Colorful_Title,
                I18n.Gmcm_Cmbt_Rings_Resonance_Colorful_Desc,
                config => config.Combat.ColorfulResonances,
                (config, value) =>
                {
                    config.Combat.ColorfulResonances = value;
                    Game1.player.Get_ResonatingChords().ForEach(chord => chord.ResetLightSource());
                })
            .AddDropdown(
                I18n.Gmcm_Cmbt_Rings_Resonance_LightSource_Title,
                I18n.Gmcm_Cmbt_Rings_Resonance_LightSource_Desc,
                config => config.Combat.ResonanceLightsourceTexture.ToString(),
                (config, value) =>
                {
                    config.Combat.ResonanceLightsourceTexture = Enum.Parse<Config.LightsourceTexture>(value);
                    Game1.player.Get_ResonatingChords().ForEach(chord => chord.ResetLightSource());
                },
                new[] { "Sconce", "Stronger", "Patterned" },
                value => _I18n.Get("gmcm.cmbt.rings.resonance.light_source." + value.ToLowerInvariant()))

            #endregion rings

            #region quests

            .AddPage(OverhaulModule.Combat.Namespace + "/Quests", I18n.Gmcm_Cmbt_Quests_Heading)
            .AddPageLink(OverhaulModule.Combat.Namespace, I18n.Gmcm_Cmbt_Back)
            .AddVerticalSpace()
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Quests_WoodyReplacesRusty_Title,
                I18n.Gmcm_Cmbt_Quests_WoodyReplacesRusty_Desc,
                config => config.Combat.WoodyReplacesRusty,
                (config, value) => config.Combat.WoodyReplacesRusty = value)
            .SetTitleScreenOnlyForNextOptions(true)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Quests_DwarvishLegacy_Title,
                I18n.Gmcm_Cmbt_Quests_DwarvishLegacy_Desc,
                config => config.Combat.DwarvenLegacy,
                (config, value) =>
                {
                    if (value && JsonAssetsIntegration.Instance?.IsLoaded != true)
                    {
                        Log.W(
                            "Cannot enable Dwarven Legacy because this feature requires Json Assets, which is not installed.");
                        return;
                    }

                    config.Combat.DwarvenLegacy = value;
                    if (value && (JsonAssetsIntegration.Instance?.IsRegistered != true ||
                                  (Context.IsWorldReady && !JsonAssetsIntegration.DwarvenScrapIndex.HasValue)))
                    {
                        Log.E("Cannot enable Dwarven Legacy because the Json Assets integration was not registered.");
                    }

                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Quests");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
                })
            .AddParagraph(I18n.Gmcm_Cmbt_Quests_DwarvishLegacy_Disclaimer)
            .SetTitleScreenOnlyForNextOptions(false)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Cmbt_Quests_InfinityPlusOne_Heading)
            .SetTitleScreenOnlyForNextOptions(true)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Quests_EnableHeroQuest_Title,
                I18n.Gmcm_Cmbt_Quests_EnableHeroQuest_Desc,
                config => config.Combat.EnableHeroQuest,
                (config, value) =>
                {
                    if (value && JsonAssetsIntegration.Instance?.IsLoaded != true)
                    {
                        Log.W(
                            "Cannot enable Hero Quest because this feature requires Json Assets, which is not installed.");
                        return;
                    }

                    config.Combat.EnableHeroQuest = value;
                    if (value && (JsonAssetsIntegration.Instance?.IsRegistered != true ||
                                  (Context.IsWorldReady && !JsonAssetsIntegration.HeroSoulIndex.HasValue)))
                    {
                        Log.E("Cannot enable Hero Quest because the Json Assets integration was not registered.");
                    }

                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/WizardHouse");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/StringsFromCSFiles");
                    ModHelper.GameContent.InvalidateCache("TileSheets/Projectiles");
                    if (VanillaTweaksIntegration.Instance?.IsRegistered == true ||
                        SimpleWeaponsIntegration.Instance?.IsRegistered == true)
                    {
                        ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
                    }
                })
            .AddParagraph(I18n.Gmcm_Cmbt_Quests_EnableHeroQuest_Disclaimer)
            .SetTitleScreenOnlyForNextOptions(false)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Quests_IridiumBarsPerGalaxyWeapon_Title,
                I18n.Gmcm_Cmbt_Quests_IridiumBarsPerGalaxyWeapon_Desc,
                config => config.Combat.IridiumBarsPerGalaxyWeapon,
                (config, value) => config.Combat.IridiumBarsPerGalaxyWeapon = value,
                0,
                50)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Quests_RuinBladeDotMultiplier_Title,
                I18n.Gmcm_Cmbt_Quests_RuinBladeDotMultiplier_Desc,
                config => config.Combat.RuinBladeDotMultiplier,
                (config, value) => config.Combat.RuinBladeDotMultiplier = value,
                0,
                2,
                0.05f)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Quests_CanStoreRuinBlade_Title,
                I18n.Gmcm_Cmbt_Quests_CanStoreRuinBlade_Desc,
                config => config.Combat.CanStoreRuinBlade,
                (config, value) => config.Combat.CanStoreRuinBlade = value)
            .AddDropdown(
                I18n.Gmcm_Cmbt_Quests_Difficulty_Title,
                I18n.Gmcm_Cmbt_Quests_Difficulty_Desc,
                config => config.Combat.HeroQuestDifficulty.ToString(),
                (config, value) =>
                {
                    config.Combat.HeroQuestDifficulty = Enum.Parse<Config.Difficulty>(value);
                    if (CombatModule.State.HeroQuest is { } quest)
                    {
                        Virtue.List.ForEach(virtue => quest.UpdateTrialProgress(virtue));
                    }
                },
                new[] { "Easy", "Medium", "Hard" },
                value => _I18n.Get("gmcm.cmbt.quests.difficulty." + value.ToLowerInvariant()))

            #endregion quests

            #region enemies

            .AddPage(OverhaulModule.Combat.Namespace + "/Enemies", I18n.Gmcm_Cmbt_Enemies_Heading)
            .AddPageLink(OverhaulModule.Combat.Namespace, I18n.Gmcm_Cmbt_Back)
            .AddVerticalSpace()
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Enemies_VariedEncounters_Title,
                I18n.Gmcm_Cmbt_Enemies_VariedEncounters_Desc,
                config => config.Combat.VariedEncounters,
                (config, value) => config.Combat.VariedEncounters = value)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Enemies_SpawnMultiplier_Title,
                I18n.Gmcm_Cmbt_Enemies_SpawnMultiplier_Desc,
                config => config.Combat.MonsterSpawnChanceMultiplier,
                (config, value) => config.Combat.MonsterSpawnChanceMultiplier = value,
                0.1f,
                10f)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Enemies_HealthMultiplier_Title,
                I18n.Gmcm_Cmbt_Enemies_HealthMultiplier_Desc,
                config => config.Combat.MonsterHealthMultiplier,
                (config, value) => config.Combat.MonsterHealthMultiplier = value,
                0.1f,
                10f)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Enemies_DamageMultiplier_Title,
                I18n.Gmcm_Cmbt_Enemies_DamageMultiplier_Desc,
                config => config.Combat.MonsterDamageMultiplier,
                (config, value) => config.Combat.MonsterDamageMultiplier = value,
                0.1f,
                10f)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Enemies_DefenseMultiplier_Title,
                I18n.Gmcm_Cmbt_Enemies_DefenseMultiplier_Desc,
                config => config.Combat.MonsterDefenseMultiplier,
                (config, value) => config.Combat.MonsterDefenseMultiplier = value,
                0.1f,
                10f)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Enemies_HealthSummand_Title,
                I18n.Gmcm_Cmbt_Enemies_HealthSummand_Desc,
                config => config.Combat.MonsterHealthSummand,
                (config, value) => config.Combat.MonsterHealthSummand = value,
                0,
                100)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Enemies_DamageSummand_Title,
                I18n.Gmcm_Cmbt_Enemies_DamageSummand_Desc,
                config => config.Combat.MonsterDamageSummand,
                (config, value) => config.Combat.MonsterDamageSummand = value,
                0,
                50)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Enemies_DefenseSummand_Title,
                I18n.Gmcm_Cmbt_Enemies_DefenseSummand_Desc,
                config => config.Combat.MonsterDefenseSummand,
                (config, value) => config.Combat.MonsterDefenseSummand = value,
                0,
                10)

            #endregion enemies

            #region controls & ui

            .AddPage(OverhaulModule.Combat.Namespace + "/ControlsAndUi", I18n.Gmcm_Headings_ControlsAndUi)
            .AddPageLink(OverhaulModule.Combat.Namespace, I18n.Gmcm_Cmbt_Back)
            .AddVerticalSpace()

            // controls
            .AddSectionTitle(I18n.Gmcm_Controls_Heading)
            .AddCheckbox(
                I18n.Gmcm_Controls_FaceMouseCursor_Title,
                I18n.Gmcm_Controls_FaceMouseCursor_Desc,
                config => config.Combat.FaceMouseCursor,
                (config, value) => config.Combat.FaceMouseCursor = value)
            .AddCheckbox(
                I18n.Gmcm_Controls_SlickMoves_Title,
                I18n.Gmcm_Controls_SlickMoves_Desc,
                config => config.Combat.SlickMoves,
                (config, value) => config.Combat.SlickMoves = value)
            .AddCheckbox(
                I18n.Gmcm_Controls_EnableAutoSelection_Title,
                I18n.Gmcm_Controls_EnableAutoSelection_Desc,
                config => config.Combat.EnableAutoSelection,
                (config, value) =>
                {
                    config.Combat.EnableAutoSelection = value;
                    if (!value)
                    {
                        CombatModule.State.AutoSelectableMelee = null;
                    }
                })
            .AddNumberField(
                I18n.Gmcm_Cmbt_Controls_AutoSelectionRange_Melee_Title,
                I18n.Gmcm_Cmbt_Controls_AutoSelectionRange_Melee_Desc,
                config => (int)config.Combat.MeleeAutoSelectionRange,
                (config, value) => config.Combat.MeleeAutoSelectionRange = (uint)value,
                1,
                5)
            .AddNumberField(
                I18n.Gmcm_Cmbt_Controls_AutoSelectionRange_Ranged_Title,
                I18n.Gmcm_Cmbt_Controls_AutoSelectionRange_Ranged_Desc,
                config => (int)config.Combat.RangedAutoSelectionRange,
                (config, value) => config.Combat.RangedAutoSelectionRange = (uint)value,
                1,
                10)
            .AddKeyBinding(
                I18n.Gmcm_Controls_SelectionKey_Title,
                () => I18n.Gmcm_Controls_SelectionKey_Desc(I18n.Gmcm_Cmbt_Weapon()),
                config => config.Combat.SelectionKey,
                (config, value) => config.Combat.SelectionKey = value)
            .AddColorPicker(
                I18n.Gmcm_Controls_SelectionBorderColor_Title,
                () => I18n.Gmcm_Controls_SelectionBorderColor_Desc(I18n.Gmcm_Cmbt_Weapon()),
                config => config.Combat.SelectionBorderColor,
                (config, value) => config.Combat.SelectionBorderColor = value,
                Color.Magenta,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)
            .AddHorizontalRule()

            // interface
            .AddSectionTitle(I18n.Gmcm_Ui_Heading)
            .AddCheckbox(
                I18n.Gmcm_Ui_ColorCoded_Title,
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Desc,
                config => config.Combat.ColorCodedForYourConvenience,
                (config, value) => config.Combat.ColorCodedForYourConvenience = value)
            .AddColorPicker(
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Common_Title,
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Common_Desc,
                config => config.Combat.CommonTierColor,
                (config, value) => config.Combat.CommonTierColor = value,
                Game1.textColor,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RadioChooser)
            .AddColorPicker(
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Uncommon_Title,
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Uncommon_Desc,
                config => config.Combat.UncommonTierColor,
                (config, value) => config.Combat.UncommonTierColor = value,
                Color.Green,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RadioChooser)
            .AddColorPicker(
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Rare_Title,
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Rare_Desc,
                config => config.Combat.RareTierColor,
                (config, value) => config.Combat.RareTierColor = value,
                Color.Blue,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RadioChooser)
            .AddColorPicker(
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Epic_Title,
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Epic_Desc,
                config => config.Combat.EpicTierColor,
                (config, value) => config.Combat.EpicTierColor = value,
                Color.Purple,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.Default)
            .AddColorPicker(
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Mythic_Title,
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Mythic_Desc,
                config => config.Combat.MythicTierColor,
                (config, value) => config.Combat.MythicTierColor = value,
                Color.Red,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.Default)
            .AddColorPicker(
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Masterwork_Title,
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Masterwork_Desc,
                config => config.Combat.MasterworkTierColor,
                (config, value) => config.Combat.MasterworkTierColor = value,
                Color.MonoGameOrange,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.Default)
            .AddColorPicker(
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Legendary_Title,
                I18n.Gmcm_Cmbt_Ui_ColorCoded_Legendary_Desc,
                config => config.Combat.LegendaryTierColor,
                (config, value) => config.Combat.LegendaryTierColor = value,
                Color.MonoGameOrange,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.Default)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Ui_Sockets_Draw_Title,
                I18n.Gmcm_Cmbt_Ui_Sockets_Draw_Desc,
                config => config.Combat.DrawForgeSockets,
                (config, value) => config.Combat.DrawForgeSockets = value)
            .AddDropdown(
                I18n.Gmcm_Cmbt_Ui_Sockets_Style_Title,
                I18n.Gmcm_Cmbt_Ui_Sockets_Style_Desc,
                config => config.Combat.SocketStyle.ToString(),
                (config, value) =>
                {
                    config.Combat.SocketStyle = Enum.Parse<Config.ForgeSocketStyle>(value);
                    ModHelper.GameContent.InvalidateCache($"{Manifest.UniqueID}/GemstoneSockets");
                },
                new[] { "Diamond", "Round", "Iridium" },
                value => _I18n.Get("gmcm.cmbt.ui.sockets.style." + value.ToLowerInvariant()))
            .AddDropdown(
                I18n.Gmcm_Cmbt_Ui_Sockets_Pos_Title,
                I18n.Gmcm_Cmbt_Ui_Sockets_Pos_Desc,
                config => config.Combat.SocketPosition.ToString(),
                (config, value) => config.Combat.SocketPosition = Enum.Parse<Config.ForgeSocketPosition>(value),
                new[] { "Standard", "AboveSeparator" },
                value => _I18n.Get("gmcm.cmbt.ui.sockets.pos." + value.ToLowerInvariant()))
            .AddDropdown(
                I18n.Gmcm_Cmbt_Ui_Tooltips_Style_Title,
                I18n.Gmcm_Cmbt_Ui_Tooltips_Style_Desc,
                config => config.Combat.WeaponTooltipStyle.ToString(),
                (config, value) => config.Combat.WeaponTooltipStyle = Enum.Parse<Config.TooltipStyle>(value),
                new[] { "Absolute", "Relative", "Vanilla" },
                value => _I18n.Get("gmcm.cmbt.ui.tooltips.style." + value.ToLowerInvariant()))
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Ui_DrawAmmo_Title,
                I18n.Gmcm_Cmbt_Ui_DrawAmmo_Desc,
                config => config.Combat.DrawCurrentAmmo,
                (config, value) => config.Combat.DrawCurrentAmmo = value)
            .AddCheckbox(
                I18n.Gmcm_Cmbt_Ui_BullseyeCursor_Title,
                I18n.Gmcm_Cmbt_Ui_BullseyeCursor_Desc,
                config => config.Combat.BullseyeReplacesCursor,
                (config, value) => config.Combat.BullseyeReplacesCursor = value);

        #endregion controls & ui
    }
}
