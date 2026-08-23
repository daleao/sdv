namespace DaLion.Shared.Attributes;

/// <summary>Denotes a class that was used in the past but has since been deprecated.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class DeprecatedAttribute : Attribute
{
}
