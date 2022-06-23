namespace DaLion.Common.Integrations;

/// <summary>Handles integration with a given mod.</summary>
/// <remarks><see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
public interface IModIntegration
{
    /// <summary>A human-readable name for the mod.</summary>
    string Label { get; }

    /// <summary>Whether the mod is available.</summary>
    bool IsLoaded { get; }
}