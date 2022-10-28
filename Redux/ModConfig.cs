namespace DaLion.Redux;

#region using directives

using Newtonsoft.Json;

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
    public bool EnableArsenal { get; set; } = true;

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
    public Arsenal.Config Arsenal { get; internal set; } = new();

    /// <summary>Gets the Ponds module config settings.</summary>
    [JsonProperty]
    public Ponds.Config Ponds { get; internal set; } = new();

    /// <summary>Gets the Professions module config settings.</summary>
    [JsonProperty]
    public Professions.Config Professions { get; internal set; } = new();

    /// <summary>Gets the Rings module config settings.</summary>
    [JsonProperty]
    public Rings.Config Rings { get; internal set; } = new();

    /// <summary>Gets the Taxes module config settings.</summary>
    [JsonProperty]
    public Taxes.Config Taxes { get; internal set; } = new();

    /// <summary>Gets the Tools module config settings.</summary>
    [JsonProperty]
    public Tools.Config Tools { get; internal set; } = new();

    /// <summary>Gets the Tweex module config settings.</summary>
    [JsonProperty]
    public Tweex.Config Tweex { get; internal set; } = new();

#if DEBUG

    /// <summary>Gets the Debug module config settings.</summary>
    [JsonProperty]
    public Debug.Config Debug { get; internal set; } = new();

#endif

    #endregion config sub-modules
}
