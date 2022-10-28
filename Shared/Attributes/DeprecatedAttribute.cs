namespace DaLion.Shared.Attributes;

/// <summary>Specifies that a class is deprecated and should not be ignored.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class DeprecatedAttribute : Attribute
{
}
