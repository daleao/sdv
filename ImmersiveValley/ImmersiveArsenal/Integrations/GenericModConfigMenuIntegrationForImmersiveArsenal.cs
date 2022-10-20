namespace DaLion.Stardew.Arsenal.Integrations;

#region using directives

using DaLion.Common.Extensions.SMAPI;
using DaLion.Common.Integrations.GenericModConfigMenu;
using DaLion.Stardew.Arsenal.Framework.Events;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Arsenal.</summary>
internal sealed class GenericModConfigMenuIntegrationForImmersiveArsenal
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenuIntegrationForImmersiveArsenal"/> class.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    public GenericModConfigMenuIntegrationForImmersiveArsenal(
        IModRegistry modRegistry,
        IManifest manifest,
        Func<ModConfig> getConfig,
        Action reset,
        Action saveAndApply)
    {
        this._configMenu =
            new GenericModConfigMenuIntegration<ModConfig>(modRegistry, manifest, getConfig, reset, saveAndApply);
    }

    /// <summary>Register the config menu if available.</summary>
    public void Register()
    {
        // get config menu
        if (!this._configMenu.IsLoaded)
        {
            return;
        }

        // register
        this._configMenu
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
                    if (value)
                    {
                        ModEntry.Events.Enable<DriftButtonPressedEvent>();
                    }
                    else
                    {
                        ModEntry.Events.Disable<DriftButtonPressedEvent>();
                    }
                })
            .AddSectionTitle(() => "Melee Weapon Settings")
            .AddCheckbox(
                () => "Immersive Club Smash",
                () =>
                    "A club smash AoE will inflict guaranteed critical damage on burrowing enemies, but completely miss flying enemies.",
                config => config.BringBackStabbySwords,
                (config, value) => config.BringBackStabbySwords = value)
            .AddCheckbox(
                () => "DefenseImprovesParryDamage",
                () =>
                    "Improve sword parrying and defensive builds by increasing the reflected damage by 10% per defense point.",
                config => config.BringBackStabbySwords,
                (config, value) => config.BringBackStabbySwords = value)
            .AddCheckbox(
                () => "Bring Back Stabby Swords",
                () =>
                    "Replace the defensive special move of some swords with an offensive lunge move.\nAFTER DISABLING THIS SETTING YOU MUST TRASH ALL OWNED STABBING SWORDS.",
                config => config.BringBackStabbySwords,
                (config, value) => config.BringBackStabbySwords = value)
            .AddCheckbox(
                () => "Woody Replaces Rusty",
                () => "Replace the starting Rusty Sword with a Wooden Blade.",
                config => config.WoodyReplacesRusty,
                (config, value) => config.WoodyReplacesRusty = value)
            .AddCheckbox(
                () => "Infinity-Plus-One Weapons",
                () => "Replace lame Galaxy and Infinity weapons with something truly legendary.",
                config => config.InfinityPlusOneWeapons,
                (config, value) =>
                {
                    config.InfinityPlusOneWeapons = value;
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/StringsFromCSFiles");
                })
            .AddSectionTitle(() => "Enchantment Settings")
            .AddCheckbox(
                () => "Rebalanced Enchants",
                () => "Improves certain underwhelming enchantments.",
                config => config.RebalancedForges,
                (config, value) => config.RebalancedForges = value)
            .AddSectionTitle(() => "Player Settings")
            .AddCheckbox(
                () => "Remove Defense Soft Cap",
                () => "Damage mitigation should not be soft-capped at 50%.",
                config => config.RemoveFarmerDefenseSoftCap,
                (config, value) => config.RemoveFarmerDefenseSoftCap = value)
            .AddSectionTitle(() => "Monster Settings")
            .AddCheckbox(
                () => "Improve Enemy Defense",
                () => "Effectively squares the defense of enemy monsters.",
                config => config.ImprovedEnemyDefense,
                (config, value) => config.ImprovedEnemyDefense = value)
            .AddCheckbox(
                () => "Crits Ignore Defense",
                () => "Damage mitigation is skipped for critical hits.",
                config => config.CritsIgnoreDefense,
                (config, value) => config.CritsIgnoreDefense = value)
            .AddNumberField(
                () => "Monster Health Multiplier",
                () => "Increases the health of all enemies.",
                config => config.MonsterHealthMultiplier,
                (config, value) => config.MonsterHealthMultiplier = value,
                1f,
                3f)
            .AddNumberField(
                () => "Monster Damage Multiplier",
                () => "Increases the damage dealt by all enemies.",
                config => config.MonsterDamageMultiplier,
                (config, value) => config.MonsterDamageMultiplier = value,
                1f,
                3f)
            .AddNumberField(
                () => "Monster Defense Multiplier",
                () => "Increases the damage resistance of all enemies.",
                config => config.MonsterDefenseMultiplier,
                (config, value) => config.MonsterDefenseMultiplier = value,
                1f,
                3f)
            .AddCheckbox(
                () => "Varied Monster Stats",
                () => "Randomizes monster stats, subject to daily luck bias, to add variability to monster encounters.",
                config => config.VariedMonsterStats,
                (config, value) => config.VariedMonsterStats = value);
    }
}
