namespace DaLion.Overhaul.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
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

        if (!ArsenalModule.Config.InfinityPlusOne && !ArsenalModule.Config.DwarvishCrafting)
        {
            return true; // run original logic
        }

        try
        {
            switch (questionAndAnswer)
            {
                case "DarkSword_GrabIt":
                {
                    Game1.playSound("parry");
                    Game1.player.addItemByMenuIfNecessaryElseHoldUp(new MeleeWeapon(Constants.DarkSwordIndex));
                    Game1.player.mailReceived.Add("gotDarkSword");
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
        var found = Game1.player.Read(DataFields.BlueprintsFound).ParseList<int>().ToHashSet();

        if (found.Contains(Constants.ForestSwordIndex))
        {
            stock.Add(
                new MeleeWeapon(Constants.ForestSwordIndex),
                new[] { 0, int.MaxValue, Globals.ElderwoodIndex!.Value, 2 });
        }

        if (found.Contains(Constants.ElfBladeIndex))
        {
            stock.Add(
                new MeleeWeapon(Constants.ElfBladeIndex),
                new[] { 0, int.MaxValue, Globals.ElderwoodIndex!.Value, 1 });
        }

        if (found.Contains(Constants.DwarfSwordIndex))
        {
            stock.Add(
                new MeleeWeapon(Constants.DwarfSwordIndex),
                new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex!.Value, 5 });
        }

        if (found.Contains(Constants.DwarfHammerIndex))
        {
            stock.Add(
                new MeleeWeapon(Constants.DwarfHammerIndex),
                new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex!.Value, 5 });
        }

        if (found.Contains(Constants.DwarfDaggerIndex))
        {
            stock.Add(
                new MeleeWeapon(Constants.DwarfDaggerIndex),
                new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex!.Value, 3 });
        }

        if (found.Contains(Constants.DragontoothCutlassIndex))
        {
            stock.Add(
                new MeleeWeapon(Constants.DragontoothCutlassIndex),
                new[] { 0, int.MaxValue, Constants.DragonToothIndex, 10 });
        }

        if (found.Contains(Constants.DragontoothClubIndex))
        {
            stock.Add(
                new MeleeWeapon(Constants.DragontoothClubIndex),
                new[] { 0, int.MaxValue, Constants.DragonToothIndex, 10 });
        }

        if (found.Contains(Constants.DragontoothShivIndex))
        {
            stock.Add(
                new MeleeWeapon(Constants.DragontoothShivIndex),
                new[] { 0, int.MaxValue, Constants.DragonToothIndex, 7 });
        }

        return stock;
    }

    #endregion injected subroutines
}
