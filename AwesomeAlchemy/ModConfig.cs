namespace DaLion.Stardew.Alchemy;

#region using directives

using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    /// <summary>Whether to draw UI element bounding boxes.</summary>
    public bool EnableDebug { get; set; } = false;

    /// <summary>Key used by trigger UI debugging events.</summary>
    public KeybindList DebugKey { get; set; } = KeybindList.Parse("LeftControl");
}