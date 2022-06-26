namespace DaLion.Common.Integrations;

#region using directives

using Extensions.Reflection;
using StardewValley;
using System;
using System.Collections;
using System.Collections.Generic;

#endregion using directives

/// <summary>Provides functionality missing from <see cref="ISpaceCoreAPI"/>.</summary>
public static class ExtendedSpaceCoreAPI
{
    public static Func<string, object> GetCustomSkillInstance = null!;
    public static Func<Farmer, string, int> GetCustomSkillExp = null!;
    public static Func<List<KeyValuePair<string, int>>> GetCustomSkillNewLevels = null!;
    public static Action<List<KeyValuePair<string, int>>> SetCustomSkillNewLevels = null!;
    public static Func<object, string> GetSkillName = null!;
    public static Func<object, IEnumerable> GetProfessions = null!;
    public static Func<object, IEnumerable> GetProfessionsForLevels = null!;
    public static Func<object, string> GetProfessionStringId = null!;
    public static Func<object, string> GetProfessionDisplayName = null!;
    public static Func<object, string> GetProfessionDescription = null!;
    public static Func<object, int> GetProfessionVanillaId = null!;
    public static Func<object, object> GetFirstProfession = null!;
    public static Func<object, object> GetSecondProfession = null!;

    /// <summary>Whether the reflected fields have been initialized.</summary>
    public static bool Initialized { get; private set; }

    public static void Init()
    {
        GetCustomSkillInstance =
            "SpaceCore.Skills".ToType().RequireMethod("GetSkill").CompileStaticDelegate<Func<string, object>>();
        GetCustomSkillExp = "SpaceCore.Skills".ToType().RequireMethod("GetExperienceFor")
            .CompileStaticDelegate<Func<Farmer, string, int>>();
        GetCustomSkillNewLevels = "SpaceCore.Skills".ToType().RequireField("NewLevels")
            .CompileStaticFieldGetterDelegate<Func<List<KeyValuePair<string, int>>>>();
        SetCustomSkillNewLevels = "SpaceCore.Skills".ToType().RequireField("NewLevels")
            .CompileStaticFieldSetterDelegate<Action<List<KeyValuePair<string, int>>>>();
        GetSkillName = "SpaceCore.Skills+Skill".ToType().RequireMethod("GetName")
            .CompileUnboundDelegate<Func<object, string>>();
        GetProfessions = "SpaceCore.Skills+Skill".ToType().RequirePropertyGetter("Professions")
            .CompileUnboundDelegate<Func<object, IEnumerable>>();
        GetProfessionsForLevels = "SpaceCore.Skills+Skill".ToType().RequirePropertyGetter("ProfessionsForLevels")
            .CompileUnboundDelegate<Func<object, IEnumerable>>();
        GetProfessionStringId = "SpaceCore.Skills+Skill+Profession".ToType().RequirePropertyGetter("Id")
            .CompileUnboundDelegate<Func<object, string>>();
        GetProfessionDisplayName = "SpaceCore.Skills+Skill+Profession".ToType().RequireMethod("GetName")
            .CompileUnboundDelegate<Func<object, string>>();
        GetProfessionDescription = "SpaceCore.Skills+Skill+Profession".ToType().RequireMethod("GetDescription")
            .CompileUnboundDelegate<Func<object, string>>();
        GetProfessionVanillaId = "SpaceCore.Skills+Skill+Profession".ToType().RequireMethod("GetVanillaId")
            .CompileUnboundDelegate<Func<object, int>>();
        GetFirstProfession = "SpaceCore.Skills+Skill+ProfessionPair".ToType().RequirePropertyGetter("First")
            .CompileUnboundDelegate<Func<object, object>>();
        GetSecondProfession = "SpaceCore.Skills+Skill+ProfessionPair".ToType().RequirePropertyGetter("Second")
            .CompileUnboundDelegate<Func<object, object>>();

        Initialized = true;
    }
}