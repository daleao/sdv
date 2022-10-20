namespace DaLion.Common.Attributes;

/// <summary>Specifies that a class should only be available in debug mode.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class DebugOnlyAttribute : Attribute
{
}
