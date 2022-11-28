namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationAnswerDialogueActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationAnswerDialogueActionPatcher"/> class.</summary>
    internal GameLocationAnswerDialogueActionPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction));
    }

    #region harmony patches

    /// <summary>Respond to grab Dark Sword proposition + blacksmith forge.</summary>
    [HarmonyPrefix]
    private static bool GameLocationAnswerDialogueActionPrefix(ref bool __result, string? questionAndAnswer)
    {
        if (questionAndAnswer is null)
        {
            __result = false;
            return false; // don't run original logic
        }

        if (!ModEntry.Config.Arsenal.InfinityPlusOne && !ModEntry.Config.Arsenal.AncientCrafting)
        {
            return true; // run original logic
        }

        try
        {
            switch (questionAndAnswer)
            {
                case "DarkSword_GrabIt":
                {
                    Utils.GetDarkSword();
                    break;
                }

                case "DarkSword_LeaveIt":
                {
                    break;
                }

                case "Blacksmith_Forge":
                {
                    Game1.activeClickableMenu = new ShopMenu(
                        GetBlacksmithForgeStock(), 0, "ClintForge");
                    break;
                }

                default:
                {
                    return true; // run original logic
                }
            }

            __result = true;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches

    #region injected subroutines

    private static Dictionary<ISalable, int[]> GetBlacksmithForgeStock()
    {
        var stock = new Dictionary<ISalable, int[]>();
        //if (Game1.player.Read<bool>(DataFields.GotDwarfSwordBlueprint))
        {
            stock.Add(
                new MeleeWeapon(Constants.DwarfSwordIndex),
                new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex!.Value, 5 });
        }

        //if (Game1.player.Read<bool>(DataFields.GotDwarfHammerBlueprint))
        {
            stock.Add(
                new MeleeWeapon(Constants.DwarfHammerIndex),
                new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex!.Value, 5 });
        }

        //if (Game1.player.Read<bool>(DataFields.GotDwarfDaggerBlueprint))
        {
            stock.Add(
                new MeleeWeapon(Constants.DwarfDaggerIndex),
                new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex!.Value, 5 });
        }

        //if (Game1.player.Read<bool>(DataFields.GotDragontoothCutlassBlueprint))
        {
            stock.Add(
                new MeleeWeapon(Constants.DragontoothCutlassIndex),
                new[] { 0, int.MaxValue, Constants.DragonToothIndex, 10 });
        }

        //if (Game1.player.Read<bool>(DataFields.GotDragontoothClubBlueprint))
        {
            stock.Add(
                new MeleeWeapon(Constants.DragontoothClubIndex),
                new[] { 0, int.MaxValue, Constants.DragonToothIndex, 10 });
        }

        //if (Game1.player.Read<bool>(DataFields.GotDragontoothShivBlueprint))
        {
            stock.Add(
                new MeleeWeapon(Constants.DragontoothShivIndex),
                new[] { 0, int.MaxValue, Constants.DragonToothIndex, 10 });
        }

        return stock;
    }

    #endregion injected subroutines
}
