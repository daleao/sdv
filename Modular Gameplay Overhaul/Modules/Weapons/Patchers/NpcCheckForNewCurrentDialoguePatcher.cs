namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using System.Reflection;
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
        try
        {
            var player = Game1.player;
            switch (__instance.Name)
            {
                case "Clint" when player.hasQuest((int)Quest.ForgeIntro):
                    if (player.Read(DataKeys.DaysLeftTranslating, -1) > 0)
                    {
                        __instance.CurrentDialogue.Clear();
                        __instance.CurrentDialogue.Push(new Dialogue(
                            I18n.Get("dialogue.clint.blueprint.notdone"),
                            __instance));
                        return false; // don't run original logic
                    }

                    if (player.Read(DataKeys.DaysLeftTranslating, int.MaxValue) <= 0)
                    {
                        __instance.CurrentDialogue.Clear();
                        __instance.CurrentDialogue.Push(new Dialogue(
                            I18n.Get("dialogue.clint.blueprint.done"),
                            __instance));
                        player.completeQuest((int)Quest.ForgeIntro);
                        player.mailReceived.Add("clintForge");
                        player.Write(DataKeys.DaysLeftTranslating, null);
                        return false; // don't run original logic
                    }

                    break;

                case "Wizard" when player.hasQuest((int)Quest.VirtuesIntro):
                    __instance.CurrentDialogue.Clear();
                    __instance.CurrentDialogue.Push(new Dialogue(
                        I18n.Get("dialogue.wizard.curse.canthelp"),
                        __instance));
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
