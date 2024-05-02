namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("DaLion.Combat")]
internal sealed class CombatIntegration : ModIntegration<CombatIntegration, ICombatApi>
{
    /// <summary>Initializes a new instance of the <see cref="CombatIntegration"/> class.</summary>
    internal CombatIntegration()
        : base(ModHelper.ModRegistry)
    {
    }
}
