namespace DaLion.Common.Integrations.SpaceCore;

#region using directives

using Extensions.Reflection;
using StardewValley.Menus;
using System;
using System.Collections;
using System.Collections.Generic;

#endregion using directives

/// <summary>Provides functionality missing from <see cref="ISpaceCoreAPI"/>.</summary>
internal static class ExtendedSpaceCoreAPI
{
    public static readonly Lazy<Func<string, object>> GetCustomSkillInstance = new(() =>
        "SpaceCore.Skills".ToType().RequireMethod("GetSkill").CompileStaticDelegate<Func<string, object>>());

    public static readonly Lazy<Func<Farmer, string, int>> GetCustomSkillExp = new(() =>
        "SpaceCore.Skills".ToType().RequireMethod("GetExperienceFor")
            .CompileStaticDelegate<Func<Farmer, string, int>>());

    public static readonly Lazy<Func<List<KeyValuePair<string, int>>>> GetCustomSkillNewLevels = new(() =>
        "SpaceCore.Skills".ToType().RequireField("NewLevels")
            .CompileStaticFieldGetterDelegate<List<KeyValuePair<string, int>>>());

    public static readonly Lazy<Action<List<KeyValuePair<string, int>>>> SetCustomSkillNewLevels = new(() =>
        "SpaceCore.Skills".ToType().RequireField("NewLevels")
            .CompileStaticFieldSetterDelegate<List<KeyValuePair<string, int>>>());

    public static readonly Lazy<Func<object, string>> GetSkillName = new(() =>
        "SpaceCore.Skills+Skill".ToType().RequireMethod("GetName").CompileUnboundDelegate<Func<object, string>>());

    public static readonly Lazy<Func<object, IEnumerable>> GetProfessions = new(() =>
        "SpaceCore.Skills+Skill".ToType().RequirePropertyGetter("Professions")
            .CompileUnboundDelegate<Func<object, IEnumerable>>());

    public static readonly Lazy<Func<object, IEnumerable>> GetProfessionsForLevels = new(() =>
        "SpaceCore.Skills+Skill".ToType().RequirePropertyGetter("ProfessionsForLevels")
            .CompileUnboundDelegate<Func<object, IEnumerable>>());

    public static readonly Lazy<Func<object, string>> GetProfessionStringId = new(() =>
        "SpaceCore.Skills+Skill+Profession".ToType().RequirePropertyGetter("Id")
            .CompileUnboundDelegate<Func<object, string>>());

    public static readonly Lazy<Func<object, string>> GetProfessionDisplayName = new(() =>
        "SpaceCore.Skills+Skill+Profession".ToType().RequireMethod("GetName")
            .CompileUnboundDelegate<Func<object, string>>());

    public static readonly Lazy<Func<object, string>> GetProfessionDescription = new(() =>
        "SpaceCore.Skills+Skill+Profession".ToType().RequireMethod("GetDescription")
            .CompileUnboundDelegate<Func<object, string>>());

    public static readonly Lazy<Func<object, int>> GetProfessionVanillaId = new(() =>
        "SpaceCore.Skills+Skill+Profession".ToType().RequireMethod("GetVanillaId")
            .CompileUnboundDelegate<Func<object, int>>());

    public static readonly Lazy<Func<object, object>> GetFirstProfession = new(() =>
        "SpaceCore.Skills+Skill+ProfessionPair".ToType().RequirePropertyGetter("First")
            .CompileUnboundDelegate<Func<object, object>>());

    public static readonly Lazy<Func<object, object>> GetSecondProfession = new(() =>
        "SpaceCore.Skills+Skill+ProfessionPair".ToType().RequirePropertyGetter("Second")
            .CompileUnboundDelegate<Func<object, object>>());

    internal static Lazy<Func<IClickableMenu, ClickableTextureComponent>> GetNewForgeMenuLeftIngredientSpot = new(() =>
        "SpaceCore.Interface.NewForgeMenu".ToType().RequireField("leftIngredientSpot")
            .CompileUnboundFieldGetterDelegate<IClickableMenu, ClickableTextureComponent>());

    internal static Lazy<Func<IClickableMenu, int, int>> GetNewForgeMenuForgeCostAtLevel = new(() =>
        "SpaceCore.Interface.NewForgeMenu".ToType().RequireMethod("GetForgeCostAtLevel")
            .CompileUnboundDelegate<Func<IClickableMenu, int, int>>());

    internal static Lazy<Func<IClickableMenu, Item, Item, int>> GetNewForgeMenuForgeCost = new(() =>
        "SpaceCore.Interface.NewForgeMenu".ToType().RequireMethod("GetForgeCost")
            .CompileUnboundDelegate<Func<IClickableMenu, Item, Item, int>>());

    internal static Lazy<Action<IClickableMenu, Item>> SetNewForgeMenuHeldItem = new(() =>
        "SpaceCore.Interface.NewForgeMenu".ToType().RequireField("heldItem")
            .CompileUnboundFieldSetterDelegate<IClickableMenu, Item>());
}