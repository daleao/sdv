namespace DaLion.Stardew.Arsenal.Integrations;

#region using directives

using Common.Integrations.GenericModConfigMenu;
using Framework.Events;
using System;
using System.Linq;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Arsenal.</summary>
internal sealed class GenericModConfigMenuIntegrationForImmersiveArsenal
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    public GenericModConfigMenuIntegrationForImmersiveArsenal(IModRegistry modRegistry, IManifest manifest,
        Func<ModConfig> getConfig, Action reset, Action saveAndApply)
    {
        _configMenu = new(modRegistry, manifest, getConfig, reset, saveAndApply);
    }

    /// <summary>Register the config menu if available.</summary>
    public void Register()
    {
        // get config menu
        if (!_configMenu.IsLoaded)
            return;

        // register
        _configMenu
            .Register()

            .AddSectionTitle(() => "Control Settings")
            .AddCheckbox(
                () => "Face Towards Mouse Cursor",
                () =>
                    "If using mouse and keyboard, turn to face towards the current cursor position before swinging your tools.",
                config => config.FaceMouseCursor,
                (config, value) =>
                {
                    config.FaceMouseCursor = value;
                    if (value) ModEntry.Manager.Enable<ArsenalButtonPressedEvent>();
                    else ModEntry.Manager.Disable<ArsenalButtonPressedEvent>();
                }
            )

            .AddSectionTitle(() => "Melee Weapon Settings")
            .AddCheckbox(
                () => "Rebalanced Weapons",
                () => "Make weapons more unique and useful.",
                config => config.RebalancedWeapons,
                (config, value) =>
                {
                    config.RebalancedWeapons = value;
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/weapons");
                }
            )
            .AddCheckbox(
                () => "Immersive Club Smash",
                () => "A club smash AoE will inflict guaranteed critical damage on burrowing enemies, but completely miss flying enemies.",
                config => config.BringBackStabbySwords,
                (config, value) => config.BringBackStabbySwords = value
            )
            .AddCheckbox(
                () => "DefenseImprovesParryDamage",
                () => "Improve sword parrying and defensive builds by increasing the reflected damage by 10% per defense point.",
                config => config.BringBackStabbySwords,
                (config, value) => config.BringBackStabbySwords = value
            )
            .AddCheckbox(
                () => "Bring Back Stabby Swords",
                () =>
                    "Replace the defensive special move of some swords with an offensive lunge move.\nAFTER DISABLING THIS SETTING YOU MUST TRASH ALL OWNED STABBING SWORDS.",
                config => config.BringBackStabbySwords,
                (config, value) => config.BringBackStabbySwords = value
            )
            .AddCheckbox(
                () => "Woody Replaces Rusty",
                () => "Replace the starting Rusty Sword with a Wooden Blade.",
                config => config.WoodyReplacesRusty,
                (config, value) => config.WoodyReplacesRusty = value
            )
            .AddCheckbox(
                () => "Infinity Plus One Sword",
                () => "Replace lame Galaxy and Infinity weapons with something truly legendary.",
                config => config.InfinityPlusOneWeapons,
                (config, value) =>
                {
                    config.InfinityPlusOneWeapons = value;
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/ObjectInformation");
                    ModEntry.ModHelper.GameContent.InvalidateCache("Strings/Locations");
                    ModEntry.ModHelper.GameContent.InvalidateCache("Strings/StringsFromCSFiles");
                }
            )
            .AddNumberField(
                () => "Dark Sword Purification Requirement",
                () => "The Dark Sword must slay this many enemies before it can be purified.",
                config => config.RequiredKillCountToPurifyDarkSword,
                (config, value) => config.RequiredKillCountToPurifyDarkSword = value,
                0,
                1000
            )

            .AddSectionTitle(() => "Slingshot Settings")
            .AddCheckbox(
                () => "Allow Slingshot Crit",
                () => "Allows Slingshot to deal critical damage and be affected by critical modifiers.",
                config => config.AllowSlingshotCrit,
                (config, value) => config.AllowSlingshotCrit = value
            )
            .AddCheckbox(
                () => "Allow Slingshot Enchants",
                () => "Allow Slingshot to be enchanted with weapon enchantments (Prismatic Shard) at the Forge.",
                config => config.NewSlingshotEnchants,
                (config, value) => config.NewSlingshotEnchants = value
            )
            .AddCheckbox(
                () => "Allow Slingshot Forges",
                () => "Allow Slingshot to be enchanted with weapon forges (gemstones) at the Forge.",
                config => config.AllowSlingshotForges,
                (config, value) => config.AllowSlingshotForges = value
            )
            .AddCheckbox(
                () => "Allow Slingshot Special Move",
                () => "Add a new stunning smack special move for slingshots.",
                config => config.AllowSlingshotSpecialMove,
                (config, value) => config.AllowSlingshotSpecialMove = value
            )
            .AddCheckbox(
                () => "Remove Slingshot Grace Period",
                () => "Projectiles should not be useless for the first 100ms.",
                config => config.RemoveSlingshotGracePeriod,
                (config, value) => config.RemoveSlingshotGracePeriod = value
            )

            .AddSectionTitle(() => "Enchantment Settings")
            .AddCheckbox(
                () => "Rebalanced Enchants",
                () => "Improves certain underwhelming enchantments.",
                config => config.RebalancedForges,
                (config, value) => config.RebalancedForges = value
            )
            .AddDropdown(
                () => "Topaz Perk",
                () =>
                    "The stat improved by the Topaz enchantment.\nYOU MUST REMOVE ALL EXISTING TOPAZ ENCHANTMENTS BEFORE CHANGING THIS SETTING.",
                config => config.TopazPerk.ToString(),
                (config, value) => config.TopazPerk = Enum.Parse<ModConfig.Perk>(value),
                Enum.GetValues<ModConfig.Perk>().Select(p => p.ToString()).ToArray(),
                null
            )

            .AddSectionTitle(() => "Misc. Settings")
            .AddCheckbox(
                () => "Remove Defense Soft Cap",
                () => "Damage mitigation should not be soft-capped at 50%.",
                config => config.RemoveDefenseSoftCap,
                (config, value) => config.RemoveDefenseSoftCap = value
            );
    }
}