namespace DaLion.Shared.Events;

/// <summary>Specifies that a <see cref="IManagedEvent"/> should ignore its <see cref="IManagedEvent.IsEnabled"/> flag.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AlwaysEnabledEventAttribute : Attribute
{
}
