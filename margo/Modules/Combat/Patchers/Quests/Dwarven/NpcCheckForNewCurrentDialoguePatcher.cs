namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Dwarven;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class NpcCheckForNewCurrentDialoguePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NpcCheckForNewCurrentDialoguePatcher"/> class.</summary>
    internal NpcCheckForNewCurrentDialoguePatcher()
    {
        this.Target = this.RequireMethod<NPC>(nameof(NPC.checkForNewCurrentDialogue));
    }

    #region harmony patches

    /// <summary>Add special custom dialogue.</summary>
    [HarmonyPrefix]
    private static bool NpcCheckForNewCurrentDialoguePrefix(NPC __instance, ref bool __result)
    {
        if (__instance.Name != "Clint")
        {
            return true; // run original logic
        }

        try
        {
            var player = Game1.player;
            if (!player.hasQuest((int)QuestId.ForgeIntro))
            {
                return true; // run original logic
            }

            if (player.Read(DataKeys.DaysLeftTranslating, -1) > 0)
            {
                __instance.CurrentDialogue.Clear();
                __instance.CurrentDialogue.Push(new Dialogue(I18n.Dialogue_Clint_Blueprint_Notdone(), __instance));
                __result = true;
                return false; // don't run original logic
            }

            if (player.Read(DataKeys.DaysLeftTranslating, int.MaxValue) <= 0)
            {
                __instance.CurrentDialogue.Clear();
                __instance.CurrentDialogue.Push(new Dialogue(I18n.Dialogue_Clint_Blueprint_Done(), __instance));
                player.completeQuest((int)QuestId.ForgeIntro);
                player.mailReceived.Add("clintForge");
                player.Write(DataKeys.DaysLeftTranslating, null);
                __result = true;
                return false; // don't run original logic
            }

            return true; // run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
