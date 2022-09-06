namespace DaLion.Stardew.Slingshots;

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    /// <summary>Allows slingshots to deal critical damage and be affected by critical modifiers.</summary>
    public bool EnableSlingshotCrits { get; set; } = true;

    /// <summary>Enable new enchantments for slingshots, as well as some old ones..</summary>
    public bool EnableSlingshotEnchants { get; set; } = true;

    /// <summary>Allow slingshots to be enchanted with weapon forges (gemstones) at the Forge.</summary>
    public bool EnableSlingshotForges { get; set; } = true;

    /// <summary>Add new stunning smack special move for slingshots.</summary>
    /// <remarks>Does not work with Immersive Professions!</remarks>
    public bool EnableSlingshotSpecialMove { get; set; } = true;

    /// <summary>Face the current cursor position before swinging your slingshot melee.</summary>
    /// <remarks>Does not work with Immersive Professions!</remarks>
    public bool FaceMouseCursor { get; set; } = true;

    /// <summary>Projectiles should not be useless for the first 100ms.</summary>
    public bool DisableSlingshotGracePeriod { get; set; } = true;
}