namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

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
    private static bool NpcCheckForNewCurrentDialoguePrefix(NPC __instance)
    {
        var player = Game1.player;
        switch (__instance.Name)
        {
            case "Clint" when player.hasQuest(Constants.ForgeIntroQuestId):
                if (player.Read(DataFields.DaysLeftTranslating, -1) > 0)
                {
                    __instance.CurrentDialogue.Clear();
                    __instance.CurrentDialogue.Push(new Dialogue(
                        ModEntry.i18n.Get("dialogue.clint.blueprint.notdone"),
                        __instance));
                    return false; // don't run original logic
                }

                if (player.Read<bool>(DataFields.ReadyToForge))
                {
                    __instance.CurrentDialogue.Clear();
                    __instance.CurrentDialogue.Push(new Dialogue(
                        ModEntry.i18n.Get("dialogue.clint.blueprint.done"),
                        __instance));
                    player.completeQuest(Constants.ForgeIntroQuestId);
                    return false; // don't run original logic
                }

                break;

            case "Wizard" when player.hasQuest(Constants.CurseQuestId):
                __instance.CurrentDialogue.Clear();
                __instance.CurrentDialogue.Push(new Dialogue(
                    ModEntry.i18n.Get("dialogue.wizard.curse.canthelp"),
                    __instance));
                return false; // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches
}
