namespace DaLion.Shared.Integrations.GenericModConfigMenu;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[RequiresMod("jltaylor-us.GMCMOptions", "GMCM Options")]
[ImplicitIgnore]
internal sealed class GenericModConfigMenuOptionsIntegration : ModIntegration<GenericModConfigMenuOptionsIntegration, IGenericModConfigMenuOptionsApi>
{
    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenuOptionsIntegration"/> class.</summary>
    internal GenericModConfigMenuOptionsIntegration()
        : base("jltaylor-us.GMCMOptions", "GMCM Options", "1.2.0", ModHelper.ModRegistry)
    {
    }
}
