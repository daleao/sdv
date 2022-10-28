namespace DaLion.Redux.Arsenal.Slingshots;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>Configs related to <see cref="StardewValley.Tools.Slingshot"/>s.</summary>
public sealed class Config
{
    /// <summary>Gets a value indicating whether projectiles should not be useless for the first 100ms.</summary>
    [JsonProperty]
    public bool DisableGracePeriod { get; internal set; } = true;

    /// <summary>Gets a value indicating whether allows slingshots to deal critical damage and be affected by critical modifiers.</summary>
    [JsonProperty]
    public bool AllowCrits { get; internal set; } = true;

    /// <summary>Gets a value indicating whether enable new enchantments for slingshots, as well as some old ones..</summary>
    [JsonProperty]
    public bool AllowEnchants { get; internal set; } = true;

    /// <summary>Gets a value indicating whether allow slingshots to be enchanted with weapon forges (gemstones) at the Forge.</summary>
    [JsonProperty]
    public bool AllowForges { get; internal set; } = true;
}
