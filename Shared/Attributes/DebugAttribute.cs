namespace DaLion.Shared.Attributes;

/// <summary>Specifies that a class should only be available in debug mode.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class DebugAttribute : Attribute
{
}
