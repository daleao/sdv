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
        try
        {
            var player = Game1.player;
            switch (__instance.Name)
            {
                case "Wizard" when !CombatModule.State.SpokeWithWizardToday:
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
                        __instance.CurrentDialogue.Push(new Dialogue(
                            I18n.Dialogue_Wizard_Curse_Canthelp(),
                            __instance));
                        __result = true;
                        CombatModule.State.SpokeWithWizardToday = true;
                        return false; // don't run original logic
                    }

                    break;

                case "Emily" when player.spouse != "Emily" && Game1.dayOfMonth % 4 == 0 && Game1.random.NextDouble() < 1d / 3d:
                    __instance.CurrentDialogue.Push(new Dialogue(I18n.Dialogue_Emily_Gemstones_First(), __instance));
                    __instance.CurrentDialogue.Push(new Dialogue(I18n.Dialogue_Emily_Gemstones_Second(), __instance));
                    __instance.CurrentDialogue.Push(new Dialogue(I18n.Dialogue_Emily_Gemstones_Third(), __instance));
                    __instance.CurrentDialogue.Push(new Dialogue(I18n.Dialogue_Emily_Gemstones_Fourth(), __instance));
                    break;

                case "Mr. Qi" when player.craftingRecipes.TryGetValue("Iridium Band", out var crafted) && crafted > 0 &&
                                   !player.Read<bool>(DataKeys.HasMadeInfinityBand) && Game1.random.NextDouble() < 1d / 3d:
                    __instance.CurrentDialogue.Push(new Dialogue(I18n.Dialogue_Qi_Gemstones_First(), __instance));
                    __instance.CurrentDialogue.Push(new Dialogue(I18n.Dialogue_Qi_Gemstones_Second(), __instance));
                    break;
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
