namespace DaLion.Stardew.Professions.Framework.Utility;

#region using directives

using System.Collections.Generic;

#endregion using directives

internal static class Experience
{
    internal const int VANILLA_CAP_I = 15000;

    internal static int PrestigeCap => ExperienceByLevel[20];

    internal static Dictionary<int, int> ExperienceByLevel = new()
    {
        { 1, 100 },
        { 2, 380 },
        { 3, 770 },
        { 4, 1300 },
        { 5, 2150 },
        { 6, 3300 },
        { 7, 4800 },
        { 8, 6900 },
        { 9, 10000 },
        { 10, VANILLA_CAP_I },
        { 11, VANILLA_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel },
        { 12, VANILLA_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 2 },
        { 13, VANILLA_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 3 },
        { 14, VANILLA_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 4 },
        { 15, VANILLA_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 5 },
        { 16, VANILLA_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 6 },
        { 17, VANILLA_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 7 },
        { 18, VANILLA_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 8 },
        { 19, VANILLA_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 9 },
        { 20, VANILLA_CAP_I + (int)ModEntry.Config.RequiredExpPerExtendedLevel * 10 }
    };
}