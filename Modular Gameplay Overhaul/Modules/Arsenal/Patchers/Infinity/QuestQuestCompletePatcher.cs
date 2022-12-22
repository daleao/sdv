namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Infinity;

#region using directives

using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Quests;

#endregion using directives

[UsedImplicitly]
internal sealed class QuestQuestCompletePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="QuestQuestCompletePatcher"/> class.</summary>
    internal QuestQuestCompletePatcher()
    {
        this.Target = this.RequireMethod<Quest>(nameof(Quest.questComplete));
    }

    #region harmony patches

    /// <summary>Initial virtue quest completion check.</summary>
    [HarmonyPostfix]
    private static void QuestCtorPostfix(Quest __instance)
    {
        if (__instance.id.Value == Constants.VirtuesIntroQuestId)
        {
            Virtue.List.ForEach(virtue => virtue.CheckForCompletion(Game1.player));
        }
    }

    #endregion harmony patches
}
