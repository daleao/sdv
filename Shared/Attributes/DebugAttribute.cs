namespace DaLion.Shared.Attributes;

/// <summary>Specifies that an implicitly-used class should only be available in debug mode.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class DebugAttribute : Attribute
{
}
