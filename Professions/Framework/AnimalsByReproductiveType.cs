namespace DaLion.Professions.Framework;

/// <summary>Records animals by whether they get pregnant or lay eggs.</summary>
/// <param name="Mammals">Animals which get pregnant.</param>
/// <param name="EggLayers">Animals which lay eggs.</param>
internal record AnimalsByReproductiveType(string[] Mammals, string[] EggLayers)
{
}
