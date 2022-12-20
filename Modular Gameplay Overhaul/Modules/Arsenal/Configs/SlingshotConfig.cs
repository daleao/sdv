namespace DaLion.Overhaul.Modules.Arsenal.Configs;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>Configs related to <see cref="StardewValley.Tools.Slingshot"/>s.</summary>
public sealed class SlingshotConfig
{
    /// <summary>Gets a value indicating whether projectiles should not be useless for the first 100ms.</summary>
    [JsonProperty]
    public bool DisableGracePeriod { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow slingshots to deal critical damage and be affected by critical modifiers.</summary>
    [JsonProperty]
    public bool EnableCrits { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to enable new enchantments for slingshots, as well as some old ones..</summary>
    [JsonProperty]
    public bool EnableEnchants { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow slingshots to be enchanted with weapon forges (gemstones) at the Forge.</summary>
    [JsonProperty]
    public bool EnableForges { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to enable the custom slingshot stun smack special move.</summary>
    [JsonProperty]
    public bool EnableSpecialMove { get; internal set; } = true;
}
