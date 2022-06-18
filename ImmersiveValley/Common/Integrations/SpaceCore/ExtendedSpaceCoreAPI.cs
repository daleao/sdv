namespace DaLion.Common.Integrations;

#region using directives

using System.Reflection;

using Extensions.Reflection;

#endregion using directives

/// <summary>Provides functionality missing from <see cref="ISpaceCoreAPI"/>.</summary>
public static class ExtendedSpaceCoreAPI
{
    public static MethodInfo GetCustomSkillInstance = null!;
    public static MethodInfo GetCustomSkillExp = null!;
    public static FieldInfo GetCustomSkillNewLevels = null!;
    public static MethodInfo GetSkillName = null!;
    public static MethodInfo GetProfessions = null!;
    public static MethodInfo GetProfessionsForLevels = null!;
    public static MethodInfo GetProfessionStringId = null!;
    public static MethodInfo GetProfessionDisplayName = null!;
    public static MethodInfo GetProfessionDescription = null!;
    public static MethodInfo GetProfessionVanillaId = null!;
    internal static MethodInfo GetFirstProfession = null!;
    internal static MethodInfo GetSecondProfession = null!;

    /// <summary>Whether the reflected fields have been initialized.</summary>
    public static bool Initialized { get; private set; }

    public static void Init()
    {
        GetCustomSkillInstance = "SpaceCore.Skills".ToType().RequireMethod("GetSkill");
        GetCustomSkillExp = "SpaceCore.Skills".ToType().RequireMethod("GetExperienceFor");
        GetCustomSkillNewLevels = "SpaceCore.Skills".ToType().RequireField("NewLevels");
        GetSkillName = "SpaceCore.Skills+Skill".ToType().RequireMethod("GetName");
        GetProfessions = "SpaceCore.Skills+Skill".ToType().RequirePropertyGetter("Professions");
        GetProfessionsForLevels = "SpaceCore.Skills+Skill".ToType().RequirePropertyGetter("ProfessionsForLevels");
        GetProfessionStringId = "SpaceCore.Skills+Skill+Profession".ToType().RequirePropertyGetter("Id");
        GetProfessionDisplayName = "SpaceCore.Skills+Skill+Profession".ToType().RequireMethod("GetName");
        GetProfessionDescription = "SpaceCore.Skills+Skill+Profession".ToType().RequireMethod("GetDescription");
        GetProfessionVanillaId = "SpaceCore.Skills+Skill+Profession".ToType().RequireMethod("GetVanillaId");
        GetFirstProfession = "SpaceCore.Skills+Skill+ProfessionPair".ToType().RequirePropertyGetter("First");
        GetSecondProfession = "SpaceCore.Skills+Skill+ProfessionPair".ToType().RequirePropertyGetter("Second");

        Initialized = true;
    }
}