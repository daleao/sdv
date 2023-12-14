namespace DaLion.Overhaul.Modules.Professions.TreasureHunts;

/// <summary>
///     The type of <see cref="ITreasureHunt"/>; either <see cref="VanillaProfession.Scavenger"/> or
///     <see cref="VanillaProfession.Prospector"/>.
/// </summary>
public enum TreasureHuntType
{
    /// <summary>A <see cref="VanillaProfession.Scavenger"/> hunt.</summary>
    Scavenger,

    /// <summary>A <see cref="VanillaProfession.Prospector"/> hunt.</summary>
    Prospector,
}
