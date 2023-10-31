namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The collection of configs for each module.</summary>
public sealed class ModConfig
{
    private static readonly Lazy<JsonSerializerSettings> JsonSerializerSettings =
        new(() => ModHelper.Data.GetJsonSerializerSettings());

    #region module flags

    /// <summary>Gets a value indicating whether the Professions module is enabled.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public bool EnableProfessions { get; internal set; } = true;

#if DEBUG

    /// <summary>Gets a value indicating whether the Combat module is enabled.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public bool EnableCombat { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Tools module is enabled.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public bool EnableTools { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Ponds module is enabled.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public bool EnablePonds { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Taxes module is enabled.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public bool EnableTaxes { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Tweex module is enabled.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public bool EnableTweex { get; internal set; } = true;

#elif RELEASE

    /// <summary>Gets a value indicating whether the Combat module is enabled.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public bool EnableCombat { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Tools module is enabled.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public bool EnableTools { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Ponds module is enabled.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public bool EnablePonds { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Taxes module is enabled.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public bool EnableTaxes { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Tweex module is enabled.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public bool EnableTweex { get; internal set; } = true;

#endif

    #endregion module flags

    #region config sub-modules

    /// <summary>Gets the Professions module config settings.</summary>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Professions", "prfs")]
    public Modules.Professions.ProfessionConfig Professions { get; internal set; } = new();

    /// <summary>Gets the Professions module config settings.</summary>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Combat", "cmbt")]
    public Modules.Combat.CombatConfig Combat { get; internal set; } = new();

    /// <summary>Gets the Tools module config settings.</summary>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Tools", "tols")]
    public Modules.Tools.ToolConfig Tools { get; internal set; } = new();

    /// <summary>Gets the Ponds module config settings.</summary>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Ponds", "pnds")]
    public Modules.Ponds.PondConfig Ponds { get; internal set; } = new();

    /// <summary>Gets the Taxes module config settings.</summary>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Taxes", "txs")]
    public Modules.Taxes.TaxConfig Taxes { get; internal set; } = new();

    /// <summary>Gets the Tweex module config settings.</summary>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Tweex", "twx")]
    public Modules.Tweex.TweexConfig Tweex { get; internal set; } = new();

    #endregion config sub-modules

    /// <summary>Gets the key used to open the Generic Mod Config Menu directly at this mod.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public KeybindList OpenMenuKey { get; internal set; } = KeybindList.Parse("LeftShift + F12");

    /// <summary>Gets the key used to engage Debug Mode.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public KeybindList DebugKey { get; internal set; } = KeybindList.Parse("OemQuotes, OemTilde");

    /// <inheritdoc />
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, JsonSerializerSettings.Value);
    }
}
