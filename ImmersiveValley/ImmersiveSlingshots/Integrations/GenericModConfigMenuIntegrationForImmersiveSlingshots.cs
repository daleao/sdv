namespace DaLion.Stardew.Slingshots.Integrations;

#region using directives

using System;
using DaLion.Common.Integrations.GenericModConfigMenu;
using DaLion.Stardew.Slingshots.Framework.Events;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Slingshots.</summary>
internal sealed class GenericModConfigMenuIntegrationForImmersiveSlingshots
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenuIntegrationForImmersiveSlingshots"/> class.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    public GenericModConfigMenuIntegrationForImmersiveSlingshots(
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
            .AddSectionTitle(() => "Slingshot Settings")
            .AddCheckbox(
                () => "Allow Slingshot Crit",
                () => "Allows Slingshot to deal critical damage and be affected by critical modifiers.",
                config => config.EnableSlingshotCrits,
                (config, value) => config.EnableSlingshotCrits = value)
            .AddCheckbox(
                () => "Allow Slingshot Enchants",
                () => "Allow Slingshot to be enchanted with weapon enchantments (Prismatic Shard) at the Forge.",
                config => config.EnableSlingshotEnchants,
                (config, value) => config.EnableSlingshotEnchants = value)
            .AddCheckbox(
                () => "Allow Slingshot Forges",
                () => "Allow Slingshot to be enchanted with weapon forges (gemstones) at the Forge.",
                config => config.EnableSlingshotForges,
                (config, value) => config.EnableSlingshotForges = value)
            .AddCheckbox(
                () => "Allow Slingshot Special Move",
                () =>
                    "Add a new stunning smack special move for slingshots. This does nothing if Immersive Professions is installed.",
                config => config.EnableSlingshotSpecialMove,
                (config, value) => config.EnableSlingshotSpecialMove = value)
            .AddCheckbox(
                () => "Face Mouse Cursor",
                () =>
                    "Face the current cursor position before swinging your slingshot special. This does nothing if Immersive Professions is installed.",
                config => config.FaceMouseCursor,
                (config, value) => config.FaceMouseCursor = value)
            .AddCheckbox(
                () => "Remove Slingshot Grace Period",
                () => "Projectiles should not be useless for the first 100ms.",
                config => config.DisableSlingshotGracePeriod,
                (config, value) => config.DisableSlingshotGracePeriod = value);
    }
}
