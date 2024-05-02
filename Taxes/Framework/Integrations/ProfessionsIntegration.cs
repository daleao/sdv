namespace DaLion.Taxes.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("DaLion.Professions")]
internal sealed class ProfessionsIntegration : ModIntegration<ProfessionsIntegration, IProfessionsApi>
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionsIntegration"/> class.</summary>
    internal ProfessionsIntegration()
        : base(ModHelper.ModRegistry)
    {
    }
}
