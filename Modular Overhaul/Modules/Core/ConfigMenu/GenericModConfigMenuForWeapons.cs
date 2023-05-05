namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Overhaul.Modules.Weapons;
using DaLion.Overhaul.Modules.Weapons.Integrations;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.GenericModConfigMenu;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the Weapons config menu.</summary>
    private void AddWeaponOptions()
    {
        this
            .AddPage(OverhaulModule.Weapons.Namespace, I18n.Gmcm_Wpnz_Heading)

            .AddSectionTitle(I18n.Gmcm_Wpnz_Rebalance_Heading)
            .AddCheckbox(
                I18n.Gmcm_Wpnz_Rebalance_Enable_Title,
                I18n.Gmcm_Wpnz_Rebalance_Enable_Desc,
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
                I18n.Gmcm_Wpnz_Rebalance_Tooltipstyle_Title,
                I18n.Gmcm_Wpnz_Rebalance_Tooltipstyle_Desc,
                config => config.Weapons.WeaponTooltipStyle.ToString(),
                (config, value) => config.Weapons.WeaponTooltipStyle = Enum.Parse<Config.TooltipStyle>(value),
                new[] { "Absolute", "Relative" },
                value => _I18n.Get("gmcm.wpnz.tooltipstye." + value.ToLowerInvariant()))
            .AddCheckbox(
                I18n.Gmcm_Ui_Colorcodedforyourconvenience_Title,
                I18n.Gmcm_Wpnz_Rebalance_Colorcodedforyourconvenience_Desc,
                config => config.Weapons.ColorCodedForYourConvenience,
                (config, value) => config.Weapons.ColorCodedForYourConvenience = value)
            .AddCheckbox(
                I18n.Gmcm_Wpnz_Rebalance_Defenseimprovesparry_Title,
                I18n.Gmcm_Wpnz_Rebalance_Defenseimprovesparry_Desc,
                config => config.Weapons.DefenseImprovesParry,
                (config, value) => config.Weapons.DefenseImprovesParry = value)
            .AddCheckbox(
                I18n.Gmcm_Wpnz_Rebalance_Groundedclubsmash_Title,
                I18n.Gmcm_Wpnz_Rebalance_Groundedclubsmash_Desc,
                config => config.Weapons.GroundedClubSmash,
                (config, value) => config.Weapons.GroundedClubSmash = value)

            .AddSectionTitle(I18n.Gmcm_Wpnz_Stabbysword_Heading)
            .AddCheckbox(
                I18n.Gmcm_Wpnz_Stabbysword_Enablestabbyswords_Title,
                I18n.Gmcm_Wpnz_Stabbysword_Enablestabbyswords_Desc,
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
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Wpnz_Combo_Heading)
            .AddCheckbox(
                I18n.Gmcm_Wpnz_Combo_Enablecombohits_Title,
                I18n.Gmcm_Wpnz_Combo_Enablecombohits_Desc,
                config => config.Weapons.EnableComboHits,
                (config, value) => config.Weapons.EnableComboHits = value)
            .AddCheckbox(
                I18n.Gmcm_Wpnz_Combo_Swipehold_Title,
                I18n.Gmcm_Wpnz_Combo_Swipehold_Desc,
                config => config.Weapons.SwipeHold,
                (config, value) => config.Weapons.SwipeHold = value)
            .AddNumberField(
                I18n.Gmcm_Wpnz_Combo_Stabbyswordhits_Title,
                I18n.Gmcm_Wpnz_Combo_Stabbyswordhits_Desc,
                config => config.Weapons.ComboHitsPerWeapon[WeaponType.StabbingSword],
                (config, value) => config.Weapons.ComboHitsPerWeapon[WeaponType.StabbingSword] = value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Wpnz_Combo_Defenseswordhits_Title,
                I18n.Gmcm_Wpnz_Combo_Defenseswordhits_Desc,
                config => config.Weapons.ComboHitsPerWeapon[WeaponType.DefenseSword],
                (config, value) => config.Weapons.ComboHitsPerWeapon[WeaponType.DefenseSword] = value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Wpnz_Combo_Clubhits_Title,
                I18n.Gmcm_Wpnz_Combo_Clubhits_Desc,
                config => config.Weapons.ComboHitsPerWeapon[WeaponType.Club],
                (config, value) => config.Weapons.ComboHitsPerWeapon[WeaponType.Club] = value,
                0,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Wpnz_Quests_Heading)
            .AddCheckbox(
                I18n.Gmcm_Wpnz_Quests_Woodyreplacesrusty_Title,
                I18n.Gmcm_Wpnz_Quests_Woodyreplacesrusty_Desc,
                config => config.Weapons.WoodyReplacesRusty,
                (config, value) => config.Weapons.WoodyReplacesRusty = value)
            .SetTitleScreenOnlyForNextOptions(true)
            .AddCheckbox(
                I18n.Gmcm_Wpnz_Quests_Dwarvishlegacy_Title,
                I18n.Gmcm_Wpnz_Quests_Dwarvishlegacy_Desc,
                config => config.Weapons.DwarvenLegacy,
                (config, value) =>
                {
                    if (value && JsonAssetsIntegration.Instance?.IsLoaded != true)
                    {
                        Log.W("Cannot enable Dwarven Legacy because this feature requires Json Assets, which is not installed.");
                        return;
                    }

                    config.Weapons.DwarvenLegacy = value;
                    if (value && (JsonAssetsIntegration.Instance?.IsRegistered != true || !Globals.DwarvenScrapIndex.HasValue))
                    {
                        (JsonAssetsIntegration.Instance as IModIntegration)!.Register();
                    }

                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Quests");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
                })
            .AddCheckbox(
                I18n.Gmcm_Wpnz_Quests_Infinityplusone_Title,
                I18n.Gmcm_Wpnz_Quests_Infinityplusone_Desc,
                config => config.Weapons.InfinityPlusOne,
                (config, value) =>
                {
                    if (value && JsonAssetsIntegration.Instance?.IsLoaded != true)
                    {
                        Log.W("Cannot enable Infinity +1 weapons because this feature requires Json Assets, which is not installed.");
                        return;
                    }

                    config.Weapons.InfinityPlusOne = value;
                    if (value && (JsonAssetsIntegration.Instance?.IsRegistered != true || !Globals.HeroSoulIndex.HasValue))
                    {
                        (JsonAssetsIntegration.Instance as IModIntegration)!.Register();
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
                    else if (VanillaTweaksIntegration.Instance?.IsLoaded == true)
                    {
                        (VanillaTweaksIntegration.Instance as IModIntegration).Register();
                    }
                    else if (SimpleWeaponsIntegration.Instance?.IsLoaded == true)
                    {
                        (SimpleWeaponsIntegration.Instance as IModIntegration).Register();
                    }
                })
            .SetTitleScreenOnlyForNextOptions(false)
            .AddNumberField(
                I18n.Gmcm_Wpnz_Quests_Iridiumbarspergalaxyweapon_Title,
                I18n.Gmcm_Wpnz_Quests_Iridiumbarspergalaxyweapon_Desc,
                config => config.Weapons.IridiumBarsPerGalaxyWeapon,
                (config, value) => config.Weapons.IridiumBarsPerGalaxyWeapon = value,
                0,
                50)
            .AddNumberField(
                I18n.Gmcm_Wpnz_Quests_Ruinbladedotmultiplier_Title,
                I18n.Gmcm_Wpnz_Quests_Ruinbladedotmultiplier_Desc,
                config => config.Weapons.RuinBladeDotMultiplier,
                (config, value) => config.Weapons.RuinBladeDotMultiplier = value,
                0,
                2,
                0.05f)
            .AddCheckbox(
                I18n.Gmcm_Wpnz_Quests_Canstoreruinblade_Title,
                I18n.Gmcm_Wpnz_Quests_Canstoreruinblade_Desc,
                config => config.Weapons.CanStoreRuinBlade,
                (config, value) => config.Weapons.CanStoreRuinBlade = value)
            .AddDropdown(
                I18n.Gmcm_Wpnz_Quests_Virtuetrialdifficulty_Title,
                I18n.Gmcm_Wpnz_Quests_Virtuetrialdifficulty_Desc,
                config => config.Weapons.VirtueTrialDifficulty.ToString(),
                (config, value) =>
                {
                    config.Weapons.VirtueTrialDifficulty = Enum.Parse<Config.TrialDifficulty>(value);
                    if (WeaponsModule.State.VirtuesQuest is { } quest)
                    {
                        Virtue.List.ForEach(virtue => quest.UpdateVirtueProgress(virtue));
                    }
                },
                new[] { "Easy", "Medium", "Hard" },
                value => _I18n.Get("gmcm.wpnz.virtuetrialdifficulty." + value.ToLowerInvariant()))
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Controls_Heading)
            .AddCheckbox(
                I18n.Gmcm_Controls_Facemousecursor_Title,
                I18n.Gmcm_Controls_Facemousecursor_Desc,
                config => config.Weapons.FaceMouseCursor,
                (config, value) => config.Weapons.FaceMouseCursor = value)
            .AddCheckbox(
                I18n.Gmcm_Controls_Enableautoselection_Title,
                () => I18n.Gmcm_Controls_Enableautoselection_Desc(I18n.Gmcm_Wpnz_Weapon().ToLowerInvariant()),
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
                I18n.Gmcm_Controls_Selectionkey_Title,
                () => I18n.Gmcm_Controls_Selectionkey_Desc(I18n.Gmcm_Wpnz_Weapon().ToLowerInvariant()),
                config => config.Weapons.SelectionKey,
                (config, value) => config.Weapons.SelectionKey = value)
            .AddColorPicker(
                I18n.Gmcm_Controls_Selectionbordercolor_Title,
                () => I18n.Gmcm_Controls_Selectionbordercolor_Desc(I18n.Gmcm_Wpnz_Weapon().ToLowerInvariant()),
                config => config.Weapons.SelectionBorderColor,
                (config, value) => config.Weapons.SelectionBorderColor = value,
                Color.Magenta,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)
            .AddNumberField(
                I18n.Gmcm_Controls_Autoselectionrange_Title,
                () => I18n.Gmcm_Controls_Autoselectionrange_Desc(I18n.Gmcm_Wpnz_Weapon().ToLowerInvariant()),
                config => (int)config.Weapons.AutoSelectionRange,
                (config, value) => config.Weapons.AutoSelectionRange = (uint)value,
                1,
                3)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Movement_Heading)
            .AddCheckbox(
                I18n.Gmcm_Movement_Slickmoves_Title,
                I18n.Gmcm_Movement_Slickmoves_Desc,
                config => config.Weapons.SlickMoves,
                (config, value) => config.Weapons.SlickMoves = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Other_Heading)
            .AddCheckbox(
                I18n.Gmcm_Wpnz_Enableretexture_Title,
                I18n.Gmcm_Wpnz_Enableretexture_Desc,
                config => config.Weapons.EnableRetexture,
                (config, value) =>
                {
                    config.Weapons.EnableRetexture = value;
                    ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
                });
    }
}
