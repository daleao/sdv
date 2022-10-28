namespace DaLion.Shared.Events;

/// <summary>Specifies that a class should only be available in debug mode.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AlwaysEnabledAttribute : Attribute
{
}
