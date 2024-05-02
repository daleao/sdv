namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Tells the GMCM generator to ignore a property.</summary>
[AttributeUsage(AttributeTargets.Property)]
internal sealed class GMCMIgnoreAttribute : Attribute
{
}
