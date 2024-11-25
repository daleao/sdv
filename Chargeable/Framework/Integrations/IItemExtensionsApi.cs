namespace DaLion.Chargeable.Framework.Integrations;

public interface IItemExtensionsApi
{
    /// <summary>Checks for resource data with the Stone type.</summary>
    /// <param name="id">Qualified item ID.</param>
    /// <returns>Whether the object has resource data with the Stone type.</returns>
    bool IsStone(string id);
}
