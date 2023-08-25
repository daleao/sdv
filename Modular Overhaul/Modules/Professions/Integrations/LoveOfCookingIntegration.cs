namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.Custom;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.LoveOfCooking;

#endregion using directives

[ModRequirement("blueberry.LoveOfCooking", "Love Of Cooking", "1.0.27")]
internal sealed class LoveOfCookingIntegration : ModIntegration<LoveOfCookingIntegration, ICookingSkillApi>
{
    /// <summary>Initializes a new instance of the <see cref="LoveOfCookingIntegration"/> class.</summary>
    internal LoveOfCookingIntegration()
        : base("blueberry.LoveOfCooking", "Love Of Cooking", "1.0.27", ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        EventManager.Enable<ProfessionLateLoadOneSecondUpdateTickedEvent>();
        return base.RegisterImpl();
    }
}
