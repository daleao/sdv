namespace DaLion.Shared.Attributes;

/// <summary>Indicates that an implicitly-used marked symbol should be ignored unless explicitly instantiated.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ImplicitIgnoreAttribute : Attribute
{
}
