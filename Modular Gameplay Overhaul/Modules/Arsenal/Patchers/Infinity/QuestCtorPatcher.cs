namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Infinity;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Quests;

#endregion using directives

[UsedImplicitly]
internal sealed class QuestCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="QuestCtorPatcher"/> class.</summary>
    internal QuestCtorPatcher()
    {
        this.Target = this.RequireConstructor<Quest>();
    }

    #region harmony patches

    /// <summary>Add individual virtue sidequests.</summary>
    [HarmonyPostfix]
    private static void QuestCtorPostfix(Quest __instance)
    {
        if (__instance.id.Value != Constants.VirtuesIntroQuestId)
        {
            return;
        }

        __instance.nextQuests.AddRange(new int[]
        {
            Virtue.Honor,
            Virtue.Compassion,
            Virtue.Wisdom,
            Virtue.Generosity,
            Virtue.Valor,
        });
    }

    #endregion harmony patches
}
