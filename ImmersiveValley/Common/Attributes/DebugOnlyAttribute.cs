namespace DaLion.Common.Attributes;

#region using directives

using System;

#endregion using directives

/// <summary>Specifies that a class should only be available in debug mode.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class DebugOnlyAttribute : Attribute
{
}