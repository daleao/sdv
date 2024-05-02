namespace DaLion.Shared.Integrations.GMCM.Attributes;

#region using directives

using DaLion.Shared.Extensions.Reflection;

#endregion using directives

/// <summary>Tells the GMCM generator to override its default generation with a specific callback when adding a property.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMOverrideAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMOverrideAttribute"/> class that depends on a condition retrieved from a property.</summary>
    /// <param name="overrideType">The <see cref="Type"/> which holds the override method.</param>
    /// <param name="overrideName">The name of the override method.</param>
    public GMCMOverrideAttribute(Type overrideType, string overrideName)
    {
        this.Override = overrideType.RequireMethod(overrideName).CompileStaticDelegate<Action>();
    }

    /// <summary>Gets a delegate that should be called to add the config property.</summary>
    public Action Override { get; }
}
