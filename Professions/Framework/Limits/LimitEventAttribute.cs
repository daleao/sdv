namespace DaLion.Professions.Framework.Limits;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>Qualifies a <see cref="ManagedEvent"/> class related to Limit Break functionality.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class LimitEventAttribute : Attribute
{
}
