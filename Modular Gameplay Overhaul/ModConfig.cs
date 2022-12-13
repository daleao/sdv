namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Arsenal;
using DaLion.Overhaul.Modules.Ponds;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Rings;
using DaLion.Overhaul.Modules.Taxes;
using DaLion.Overhaul.Modules.Tools;
using DaLion.Overhaul.Modules.Tweex;
using DaLion.Shared.Configs;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The core mod user-defined settings.</summary>
public sealed class ModConfig
{
    #region module flags

    /// <summary>Gets a value indicating whether the Professions module is enabled.</summary>
    [JsonProperty]
    public bool EnableProfessions { get; internal set; } = true;

#if DEBUG

    /// <summary>Gets a value indicating whether the Arsenal module is enabled.</summary>
    [JsonProperty]
    public bool EnableArsenal { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Ponds module is enabled.</summary>
    [JsonProperty]
    public bool EnablePonds { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Rings module is enabled.</summary>
    [JsonProperty]
    public bool EnableRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Taxes module is enabled.</summary>
    [JsonProperty]
    public bool EnableTaxes { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Tools module is enabled.</summary>
    [JsonProperty]
    public bool EnableTools { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Tweex module is enabled.</summary>
    [JsonProperty]
    public bool EnableTweex { get; internal set; } = true;

#elif RELEASE

    /// <summary>Gets a value indicating whether the Arsenal module is enabled.</summary>
    [JsonProperty]
    public bool EnableArsenal { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Ponds module is enabled.</summary>
    [JsonProperty]
    public bool EnablePonds { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Rings module is enabled.</summary>
    [JsonProperty]
    public bool EnableRings { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Slingshots module is enabled.</summary>
    [JsonProperty]
    public bool EnableSlingshots { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Taxes module is enabled.</summary>
    [JsonProperty]
    public bool EnableTaxes { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Tools module is enabled.</summary>
    [JsonProperty]
    public bool EnableTools { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Tweex module is enabled.</summary>
    [JsonProperty]
    public bool EnableTweex { get; internal set; } = true;

#endif

    #endregion module flags

    #region config sub-modules

    /// <summary>Gets the Arsenal module config settings.</summary>
    [JsonProperty]
    public ArsenalConfig Arsenal { get; internal set; } = new();

    /// <summary>Gets the Ponds module config settings.</summary>
    [JsonProperty]
    public PondsConfig Ponds { get; internal set; } = new();

    /// <summary>Gets the Professions module config settings.</summary>
    [JsonProperty]
    public ProfessionsConfig Professions { get; internal set; } = new();

    /// <summary>Gets the Rings module config settings.</summary>
    [JsonProperty]
    public RingsConfig Rings { get; internal set; } = new();

    /// <summary>Gets the Taxes module config settings.</summary>
    [JsonProperty]
    public TaxesConfig Taxes { get; internal set; } = new();

    /// <summary>Gets the Tools module config settings.</summary>
    [JsonProperty]
    public ToolsConfig Tools { get; internal set; } = new();

    /// <summary>Gets the Tweex module config settings.</summary>
    [JsonProperty]
    public TweexConfig Tweex { get; internal set; } = new();

    #endregion config sub-modules

    /// <summary>Gets the key used to trigger debug features.</summary>
    [JsonProperty]
    public KeybindList DebugKey { get; internal set; } = KeybindList.Parse("Tab");

    /// <summary>Validates all internal configs and overwrites the user's config file if any invalid settings were found.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    internal void Validate(IModHelper helper)
    {
        if (!this.List().Aggregate(true, (flag, config) => flag | config.Validate()))
        {
            helper.WriteConfig(this);
        }
    }

    private IEnumerable<Config> List()
    {
        yield return this.Arsenal;
        yield return this.Professions;
        yield return this.Rings;
        yield return this.Ponds;
        yield return this.Taxes;
        yield return this.Tools;
        yield return this.Tweex;
    }
}
