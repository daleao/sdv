namespace DaLion.Combat;

/// <summary>The <see cref="CombatMod"/> API implementation.</summary>
public class CombatApi : ICombatApi
{
    /// <inheritdoc />
    public CombatConfig GetConfig()
    {
        return Config;
    }
}
