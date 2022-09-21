namespace DaLion.Common.Integrations.SpaceCore;

#region using directives

using System;
using System.Collections;
using System.Collections.Generic;
using DaLion.Common.Extensions.Reflection;
using StardewValley.Menus;

#endregion using directives

/// <summary>Provides functionality missing from <see cref="ISpaceCoreApi"/>.</summary>
internal static class ExtendedSpaceCoreApi
{
    #region custom skills

    internal static Lazy<Func<string, object>> GetCustomSkillInstance { get; } = new(() =>
        "SpaceCore.Skills"
            .ToType()
            .RequireMethod("GetSkill")
            .CompileStaticDelegate<Func<string, object>>());

    internal static Lazy<Func<Farmer, string, int>> GetCustomSkillExp { get; } = new(() =>
        "SpaceCore.Skills"
            .ToType()
            .RequireMethod("GetExperienceFor")
            .CompileStaticDelegate<Func<Farmer, string, int>>());

    internal static Lazy<Func<List<KeyValuePair<string, int>>>> GetCustomSkillNewLevels { get; } = new(() =>
        "SpaceCore.Skills"
            .ToType()
            .RequireField("NewLevels")
            .CompileStaticFieldGetterDelegate<List<KeyValuePair<string, int>>>());

    internal static Lazy<Action<List<KeyValuePair<string, int>>>> SetCustomSkillNewLevels { get; } = new(() =>
        "SpaceCore.Skills"
            .ToType()
            .RequireField("NewLevels")
            .CompileStaticFieldSetterDelegate<List<KeyValuePair<string, int>>>());

    internal static Lazy<Func<object, string>> GetSkillName { get; } = new(() =>
        "SpaceCore.Skills+Skill"
            .ToType()
            .RequireMethod("GetName")
            .CompileUnboundDelegate<Func<object, string>>());

    internal static Lazy<Func<object, IEnumerable>> GetProfessions { get; } = new(() =>
        "SpaceCore.Skills+Skill"
            .ToType()
            .RequirePropertyGetter("Professions")
            .CompileUnboundDelegate<Func<object, IEnumerable>>());

    internal static Lazy<Func<object, IEnumerable>> GetProfessionsForLevels { get; } = new(() =>
        "SpaceCore.Skills+Skill"
            .ToType()
            .RequirePropertyGetter("ProfessionsForLevels")
            .CompileUnboundDelegate<Func<object, IEnumerable>>());

    internal static Lazy<Func<object, string>> GetProfessionStringId { get; } = new(() =>
        "SpaceCore.Skills+Skill+Profession"
            .ToType()
            .RequirePropertyGetter("Id")
            .CompileUnboundDelegate<Func<object, string>>());

    internal static Lazy<Func<object, string>> GetProfessionDisplayName { get; } = new(() =>
        "SpaceCore.Skills+Skill+Profession"
            .ToType()
            .RequireMethod("GetName")
            .CompileUnboundDelegate<Func<object, string>>());

    internal static Lazy<Func<object, string>> GetProfessionDescription { get; } = new(() =>
        "SpaceCore.Skills+Skill+Profession"
            .ToType()
            .RequireMethod("GetDescription")
            .CompileUnboundDelegate<Func<object, string>>());

    internal static Lazy<Func<object, int>> GetProfessionVanillaId { get; } = new(() =>
        "SpaceCore.Skills+Skill+Profession"
            .ToType()
            .RequireMethod("GetVanillaId")
            .CompileUnboundDelegate<Func<object, int>>());

    internal static Lazy<Func<object, object>> GetFirstProfession { get; } = new(() =>
        "SpaceCore.Skills+Skill+ProfessionPair"
            .ToType()
            .RequirePropertyGetter("First")
            .CompileUnboundDelegate<Func<object, object>>());

    internal static Lazy<Func<object, object>> GetSecondProfession { get; } = new(() =>
        "SpaceCore.Skills+Skill+ProfessionPair"
            .ToType()
            .RequirePropertyGetter("Second")
            .CompileUnboundDelegate<Func<object, object>>());

    #endregion custom skills

    #region ui

    internal static Lazy<Func<IClickableMenu, ClickableTextureComponent>> GetNewForgeMenuLeftIngredientSpot { get; } = new(() =>
        "SpaceCore.Interface.NewForgeMenu"
            .ToType()
            .RequireField("leftIngredientSpot")
            .CompileUnboundFieldGetterDelegate<IClickableMenu, ClickableTextureComponent>());

    internal static Lazy<Func<IClickableMenu, int, int>> GetNewForgeMenuForgeCostAtLevel { get; } = new(() =>
        "SpaceCore.Interface.NewForgeMenu"
            .ToType()
            .RequireMethod("GetForgeCostAtLevel")
            .CompileUnboundDelegate<Func<IClickableMenu, int, int>>());

    internal static Lazy<Func<IClickableMenu, Item, Item, int>> GetNewForgeMenuForgeCost { get; } = new(() =>
        "SpaceCore.Interface.NewForgeMenu"
            .ToType()
            .RequireMethod("GetForgeCost")
            .CompileUnboundDelegate<Func<IClickableMenu, Item, Item, int>>());

    internal static Lazy<Action<IClickableMenu, Item>> SetNewForgeMenuHeldItem { get; } = new(() =>
        "SpaceCore.Interface.NewForgeMenu"
            .ToType()
            .RequireField("heldItem")
            .CompileUnboundFieldSetterDelegate<IClickableMenu, Item>());

    #endregion ui
}
