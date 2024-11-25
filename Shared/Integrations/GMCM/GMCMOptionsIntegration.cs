namespace DaLion.Shared.Integrations.GMCM;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives


[ModRequirement("jltaylor-us.GMCMOptions", "GMCM Options", "1.6.0")]
public sealed class GMCMOptionsIntegration : ModIntegration<GMCMOptionsIntegration, IGenericModConfigMenuOptionsApi>
{
    /// <summary>Initializes a new instance of the <see cref="GMCMOptionsIntegration"/> class.</summary>
    public GMCMOptionsIntegration()
        : base(ModHelper.ModRegistry)
    {
    }
}
