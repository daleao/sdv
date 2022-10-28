namespace DaLion.Stardew.Slingshots.Integrations;

#region using directives

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
                () => "Allow Crits",
                () => "Allows Slingshot to deal critical damage and be affected by critical modifiers.",
                config => config.AllowCrits,
                (config, value) => config.AllowCrits = value)
            .AddCheckbox(
                () => "Allow Enchants",
                () => "Allow Slingshot to be enchanted with weapon enchantments (Prismatic Shard) at the Forge.",
                config => config.AllowEnchants,
                (config, value) => config.AllowEnchants = value)
            .AddCheckbox(
                () => "Allow Forges",
                () => "Allow Slingshot to be enchanted with weapon forges (gemstones) at the Forge.",
                config => config.AllowForges,
                (config, value) => config.AllowForges = value)
            .AddCheckbox(
                () => "Remove Grace Period",
                () => "Projectiles should not be useless for the first 100ms.",
                config => config.DisableGracePeriod,
                (config, value) => config.DisableGracePeriod = value);
    }
}
