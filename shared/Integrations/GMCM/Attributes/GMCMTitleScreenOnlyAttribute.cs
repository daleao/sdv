namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Tells the GMCM generator that a property should only appear in the title screen of the menu.</summary>
[AttributeUsage(AttributeTargets.Property)]
internal sealed class GMCMTitleScreenOnlyAttribute : Attribute
{
}
