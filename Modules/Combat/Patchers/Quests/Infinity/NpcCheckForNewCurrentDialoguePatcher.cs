namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Infinity;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Extensions;
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
        if (__instance.Name != "Wizard" || CombatModule.State.SpokeWithWizardToday)
        {
            return true; // run original logic
        }

        try
        {
            var player = Game1.player;
            if (player.IsCursed(out var darkSword) && player.eventsSeen.Contains((int)QuestId.CurseIntro) &&
                darkSword.Read<int>(DataKeys.CursePoints) >= 100)
            {
                __instance.CurrentDialogue.Push(new Dialogue(I18n.Dialogue_Wizard_Curse_Toldya(), __instance));
                CombatModule.State.SpokeWithWizardToday = true;
                __result = true;
                return false; // don't run original logic
            }

            if (player.hasQuest((int)QuestId.CurseIntro))
            {
                __instance.CurrentDialogue.Push(new Dialogue(I18n.Dialogue_Wizard_Curse_Canthelp(), __instance));
                __result = true;
                CombatModule.State.SpokeWithWizardToday = true;
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
