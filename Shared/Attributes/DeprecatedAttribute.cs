namespace DaLion.Shared.Attributes;

/// <summary>Specifies that an implicitly-used class is deprecated and should not be instantiated.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class DeprecatedAttribute : Attribute
{
}
