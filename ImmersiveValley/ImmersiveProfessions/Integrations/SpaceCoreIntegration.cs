#nullable enable
namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System;
using System.Reflection;
using StardewModdingAPI;

using Common.Extensions.Reflection;
using Common.Integrations;

#endregion using directives

internal class SpaceCoreIntegration : BaseIntegration<ISpaceCoreAPI>
{
    internal static MethodInfo GetCustomSkill = null!;
    internal static MethodInfo GetCustomSkillExp = null!;
    internal static FieldInfo GetCustomSkillNewLevels = null!;
    internal static MethodInfo GetSkillName = null!;
    internal static MethodInfo GetProfessions = null!;
    internal static MethodInfo GetProfessionsForLevels = null!;
    internal static MethodInfo GetPairLevel = null!;
    internal static MethodInfo GetFirstProfession = null!;
    internal static MethodInfo GetSecondProfession = null!;
    internal static MethodInfo GetRequiredProfession = null!;
    internal static MethodInfo GetProfessionName = null!;
    internal static MethodInfo GetProfessionStringId = null!;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="log">Encapsulates monitoring and logging.</param>
    public SpaceCoreIntegration(IModRegistry modRegistry, Action<string, LogLevel> log)
        : base("SpaceCore", "spacechase0.SpaceCore", "1.8.3", modRegistry, log)
    {
    }

    /// <summary>Cache the SpaceCore API.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.SpaceCoreApi = ModApi;
    }

    /// <summary>Cache reflected methods not exposed by SpaceCore's API.</summary>
    public static void InitializeReflectedFields()
    {
        GetCustomSkill = "SpaceCore.Skills".ToType().RequireMethod("GetSkill");
        GetCustomSkillExp = "SpaceCore.Skills".ToType().RequireMethod("GetExperienceFor");
        GetCustomSkillNewLevels = "SpaceCore.Skills".ToType().RequireField("NewLevels");
        GetSkillName = "SpaceCore.Skills+Skill".ToType().RequireMethod("GetName");
        GetProfessions = "SpaceCore.Skills+Skill".ToType().RequirePropertyGetter("Professions");
        GetProfessionsForLevels = "SpaceCore.Skills+Skill".ToType().RequirePropertyGetter("ProfessionsForLevels");
        GetPairLevel = "SpaceCore.Skills+Skill+ProfessionPair".ToType().RequirePropertyGetter("Level");
        GetFirstProfession = "SpaceCore.Skills+Skill+ProfessionPair".ToType().RequirePropertyGetter("First");
        GetSecondProfession = "SpaceCore.Skills+Skill+ProfessionPair".ToType().RequirePropertyGetter("Second");
        GetRequiredProfession = "SpaceCore.Skills+Skill+ProfessionPair".ToType().RequirePropertyGetter("Requires");
        GetProfessionName = "SpaceCore.Skills+Skill+Profession".ToType().RequireMethod("GetName");
        GetProfessionStringId = "SpaceCore.Skills+Skill+Profession".ToType().RequirePropertyGetter("Id");
    }
}