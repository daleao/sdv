namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.LoveOfCooking;

#endregion using directives

[RequiresMod("blueberry.LoveOfCooking", "Love Of Cooking", "1.0.27")]
internal sealed class LoveOfCookingIntegration : ModIntegration<LoveOfCookingIntegration, ICookingSkillApi>
{
    private LoveOfCookingIntegration()
        : base("blueberry.LoveOfCooking", "Love Of Cooking", "1.0.27", ModHelper.ModRegistry)
    {
    }
}
