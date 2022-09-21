namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using System;
using System.Reflection;
using DaLion.Common;
using HarmonyLib;
using StardewValley.Menus;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;
using Utility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationAnswerDialogueActionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationAnswerDialogueActionPatch"/> class.</summary>
    internal GameLocationAnswerDialogueActionPatch()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
    [HarmonyPrefix]
    private static bool GameLocationAnswerDialogueActionPrefix(string questionAndAnswer)
    {
        try
        {
            switch (questionAndAnswer)
            {
                case "Blacksmith_Upgrade":
                    // ask to specify between tools and weapons
                    break;
                case "Blacksmith_Upgrade_Tool":
                    Game1.activeClickableMenu =
                        new ShopMenu(Utility.getBlacksmithUpgradeStock(Game1.player), 0, "ClintUpgrade");
                    break;
                case "Blacksmith_Upgrade_Weapon":
                    // launch weapon upgrade menu
                    break;
                case "Blacksmith_Dismantle":
                    // launch dismantling menu
                    break;
                default:
                    return true; // run original logic
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
