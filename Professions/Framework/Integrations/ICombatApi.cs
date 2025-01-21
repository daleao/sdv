namespace DaLion.Professions.Framework.Integrations;

/// <summary>The public interface for the Combat mod's API.</summary>
public interface ICombatApi
{
    /// <summary>The interface to the Combat mod's config settings.</summary>
    public interface ICombatConfig
    {
        /// <summary>Gets a value indicating whether to replace the linear damage mitigation formula with a rational formula that gives more impactful but diminishing returns from the defense stat.</summary>
        public bool GeometricMitigationFormula { get; }
    }

    /// <summary>Gets the mod's current config schema.</summary>
    /// <returns>The current config instance.</returns>
    ICombatConfig GetConfig();
}
