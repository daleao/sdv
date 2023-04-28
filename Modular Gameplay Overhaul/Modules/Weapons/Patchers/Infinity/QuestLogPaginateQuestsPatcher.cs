namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Quests;

#endregion using directives

[UsedImplicitly]
internal sealed class QuestLogPaginateQuestsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="QuestLogPaginateQuestsPatcher"/> class.</summary>
    internal QuestLogPaginateQuestsPatcher()
    {
        this.Target = typeof(QuestLog).RequireMethod("paginateQuests");
    }

    #region harmony patches

    /// <summary>Replace highlighting method.</summary>
    [HarmonyPostfix]
    private static void QuestLogPaginateQuestsPostfix(List<List<IQuest>> ___pages)
    {
        if (WeaponsModule.State.Quest is { } quest)
        {
            ___pages[0].Insert(0, quest);
        }
    }

    #endregion harmony patches
}
