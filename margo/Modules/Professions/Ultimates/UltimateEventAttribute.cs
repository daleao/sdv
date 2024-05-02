namespace DaLion.Overhaul.Modules.Professions.Ultimates;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>Qualifies a <see cref="ManagedEvent"/> class related to ultimate abilities / limit breaks.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class UltimateEventAttribute : Attribute
{
}
