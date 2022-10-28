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
            .AddSectionTitle(() => "Movement Settings")
            .AddCheckbox(
                () => "Face Towards Mouse Cursor",
                () =>
                    "If using mouse and keyboard, turn to face towards the current cursor position before swinging your tools.",
                config => config.FaceMouseCursor,
                (config, value) =>
                {
                    config.FaceMouseCursor = value;
                })
            .AddCheckbox(
                () => "Slick Moves",
                () =>
                    "Allows the farmer to drift when using weapons while running.",
                config => config.SlickMoves,
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

            .AddSectionTitle(() => "Weapon Settings")
            .AddCheckbox(
                () => "Woody Replaces Rusty",
                () => "Replace the starting Rusty Sword with a Wooden Blade.",
                config => config.WoodyReplacesRusty,
                (config, value) => config.WoodyReplacesRusty = value)
            .AddCheckbox(
                () => "Bring Back Stabby Swords",
                () =>
                    "Replace the defensive special move of some swords with an offensive lunge move.\nAFTER DISABLING THIS SETTING YOU MUST TRASH ALL OWNED STABBING SWORDS.",
                config => config.BringBackStabbySwords,
                (config, value) => config.BringBackStabbySwords = value)
            .AddCheckbox(
                () => "Combo Hits",
                () => "Replaces vanilla weapon spam with a more strategic combo system.",
                config => config.ComboHits,
                (config, value) =>
                {
                    config.ComboHits = value;
                    if (value)
                    {
                        ModEntry.Events.Enable<ComboButtonPressedEvent>();
                    }
                    else
                    {
                        ModEntry.Events.Disable<ComboButtonPressedEvent>();
                    }
                })
            .AddCheckbox(
                () => "DefenseImprovesParryDamage",
                () =>
                    "Improve sword parrying and defensive builds by increasing the reflected damage by 10% per defense point.",
                config => config.BringBackStabbySwords,
                (config, value) => config.BringBackStabbySwords = value)
            .AddCheckbox(
                () => "Immersive Club Smash",
                () =>
                    "A club smash AoE will inflict guaranteed critical damage on burrowing enemies, but completely miss flying enemies.",
                config => config.BringBackStabbySwords,
                (config, value) => config.BringBackStabbySwords = value)
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
                () => "Overhauled Enchants",
                () => "Replaces boring old enchantments with exciting new ones.",
                config => config.OverhauledEnchants,
                (config, value) => config.RebalancedForges = value)
            .AddCheckbox(
                () => "Rebalanced Forges",
                () => "Improves certain underwhelming forges (analogous with Fellowship - Immersive Rings).",
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
                () => "Improved Enemy Defense",
                () => "Effectively squares the defense of enemy monsters.",
                config => config.EnemyDefenseOverhaul,
                (config, value) => config.EnemyDefenseOverhaul = value)
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
                () => "Varied Encounters",
                () => "Randomizes monster stats, subject to daily luck bias, adding variability to monster encounters.",
                config => config.VariedEncounters,
                (config, value) => config.VariedEncounters = value);
    }
}
