namespace DaLion.Arsenal.Framework.Integrations;

/// <summary>The public interface for the Combat mod API.</summary>
public interface ICombatApi
{
    /// <summary>The config interface.</summary>
    public interface ICombatConfig
    {
        /// <summary>Gets a value indicating whether to replace the linear damage mitigation formula with a rational formula that gives more impactful but diminishing returns from the defense stat.</summary>
        public bool GeometricMitigationFormula { get; }

        /// <summary>Gets a value indicating whether defense should improve parry damage.</summary>
        public bool DefenseImprovesParry { get; }

        /// <summary>Gets a value indicating whether critical strikes should ignore the target's defense.</summary>
        public bool CritsIgnoreDefense { get; }

        /// <summary>Gets a value indicating whether back attacks gain double crit. chance.</summary>
        public bool CriticalBackAttacks { get; }
    }

    /// <summary>Gets the mod's current config schema.</summary>
    /// <returns>The current config instance.</returns>
    ICombatConfig GetConfig();
}
