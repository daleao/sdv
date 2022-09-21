namespace DaLion.Common.Integrations.LuckSkill;

#region using directives

using System.Reflection;
using DaLion.Common.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Provides functionality missing from <see cref="ILuckSkillApi"/>.</summary>
internal static class ExtendedLuckSkillApi
{
    internal static MethodInfo? GetProfessions { get; } = AccessTools.TypeByName("LuckSkill.Mod")?.RequireMethod("GetProfessions");
}
