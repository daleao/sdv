namespace DaLion.Professions.Framework.Integrations;

/// <summary>The public interface for the ItemExtensions mod API.</summary>
public interface IItemExtensionsApi
{
    /// <summary>Checks for resource data in the mod.</summary>
    /// <param name="id">Qualified item ID.</param>
    /// <param name="health">MinutesUntilReady value.</param>
    /// <param name="itemDropped">Item dropped by ore.</param>
    /// <returns>Whether the object has resource data.</returns>
    bool IsResource(string id, out int? health, out string itemDropped);
}
