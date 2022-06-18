namespace DaLion.Common.Integrations;

#region using directives

using System.Reflection;

using Extensions.Reflection;

#endregion using directives

/// <summary>Provides functionality missing from <see cref="ILuckSkillAPI"/>.</summary>
public static class ExtendedLuckSkillAPI
{
    public static MethodInfo GetProfessions = null!;

    /// <summary>Whether the reflected fields have been initialized.</summary>
    public static bool Initialized { get; private set; }

    public static void Init()
    {
        GetProfessions = "LuckSkill.Mod".ToType().RequireMethod("GetProfessions");
        Initialized = true;
    }
}