namespace DaLion.Stardew.Slingshots.Integrations;

#region using directives

using Common.Integrations.GenericModConfigMenu;
using Framework.Events;
using System;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Slingshots.</summary>
internal sealed class GenericModConfigMenuIntegrationForImmersiveSlingshots
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    public GenericModConfigMenuIntegrationForImmersiveSlingshots(IModRegistry modRegistry, IManifest manifest,
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
                    if (value) ModEntry.Events.Enable<SlingshotButtonPressedEvent>();
                    else ModEntry.Events.Disable<SlingshotButtonPressedEvent>();
                }
            )

            .AddSectionTitle(() => "Slingshot Settings")
            .AddCheckbox(
                () => "Allow Slingshot Crit",
                () => "Allows Slingshot to deal critical damage and be affected by critical modifiers.",
                config => config.EnableSlingshotCrits,
                (config, value) => config.EnableSlingshotCrits = value
            )
            .AddCheckbox(
                () => "Allow Slingshot Enchants",
                () => "Allow Slingshot to be enchanted with weapon enchantments (Prismatic Shard) at the Forge.",
                config => config.EnableSlingshotEnchants,
                (config, value) => config.EnableSlingshotEnchants = value
            )
            .AddCheckbox(
                () => "Allow Slingshot Forges",
                () => "Allow Slingshot to be enchanted with weapon forges (gemstones) at the Forge.",
                config => config.EnableSlingshotForges,
                (config, value) => config.EnableSlingshotForges = value
            )
            .AddCheckbox(
                () => "Allow Slingshot Special Move",
                () => "Add a new stunning smack special move for slingshots.",
                config => config.EnableSlingshotSpecialMove,
                (config, value) => config.EnableSlingshotSpecialMove = value
            )
            .AddCheckbox(
                () => "Remove Slingshot Grace Period",
                () => "Projectiles should not be useless for the first 100ms.",
                config => config.DisableSlingshotGracePeriod,
                (config, value) => config.DisableSlingshotGracePeriod = value
            );
    }
}