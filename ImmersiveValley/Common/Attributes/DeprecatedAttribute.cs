namespace DaLion.Common.Attributes;

#region using directives

using System;

#endregion using directives

/// <summary>Specifies that a class is deprecated and should not be available.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class DeprecatedAttribute : Attribute
{
}