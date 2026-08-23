namespace DaLion.Shared.Attributes;

/// <summary>Denotes a class that is unused or does not have an explicit purpose.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class UnusedAttributed : Attribute
{
}
