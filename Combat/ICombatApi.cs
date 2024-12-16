namespace DaLion.Combat;

/// <summary>The public interface for the Combat mod API.</summary>
public interface ICombatApi
{
    /// <summary>Gets the mod's current config schema.</summary>
    /// <returns>The current config instance.</returns>
    CombatConfig GetConfig();
}
