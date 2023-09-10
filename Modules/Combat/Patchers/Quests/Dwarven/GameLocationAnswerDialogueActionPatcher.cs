namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Dwarven;

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

        if (!CombatModule.Config.EnableHeroQuest && !CombatModule.Config.DwarvenLegacy)
        {
            return true; // run original logic
        }

        if (questionAndAnswer != "Blacksmith_Forge" || !Globals.DwarvishBlueprintIndex.HasValue)
        {
            return true; // run original logic
        }

        try
        {
            Game1.activeClickableMenu = new ShopMenu(
                GetBlacksmithForgeStock(), 0, "ClintForge");
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
        var found = Game1.player.Read(DataKeys.BlueprintsFound).ParseList<int>().ToHashSet();

        if (Globals.ElderwoodIndex.HasValue)
        {
            if (found.Contains(ItemIDs.ForestSword))
            {
                stock.Add(
                    new MeleeWeapon(ItemIDs.ForestSword),
                    new[] { 0, int.MaxValue, Globals.ElderwoodIndex.Value, 2 });
            }

            if (found.Contains(ItemIDs.ElfBlade))
            {
                stock.Add(
                    new MeleeWeapon(ItemIDs.ElfBlade),
                    new[] { 0, int.MaxValue, Globals.ElderwoodIndex.Value, 1 });
            }
        }

        if (Globals.DwarvenScrapIndex.HasValue)
        {
            if (found.Contains(ItemIDs.DwarfSword))
            {
                stock.Add(
                    new MeleeWeapon(ItemIDs.DwarfSword),
                    new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex.Value, 5 });
            }

            if (found.Contains(ItemIDs.DwarfHammer))
            {
                stock.Add(
                    new MeleeWeapon(ItemIDs.DwarfHammer),
                    new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex.Value, 5 });
            }

            if (found.Contains(ItemIDs.DwarfDagger))
            {
                stock.Add(
                    new MeleeWeapon(ItemIDs.DwarfDagger),
                    new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex.Value, 3 });
            }
        }

        if (found.Contains(ItemIDs.DragontoothCutlass))
        {
            stock.Add(
                new MeleeWeapon(ItemIDs.DragontoothCutlass),
                new[] { 0, int.MaxValue, ItemIDs.DragonTooth, 10 });
        }

        if (found.Contains(ItemIDs.DragontoothClub))
        {
            stock.Add(
                new MeleeWeapon(ItemIDs.DragontoothClub),
                new[] { 0, int.MaxValue, ItemIDs.DragonTooth, 10 });
        }

        if (found.Contains(ItemIDs.DragontoothShiv))
        {
            stock.Add(
                new MeleeWeapon(ItemIDs.DragontoothShiv),
                new[] { 0, int.MaxValue, ItemIDs.DragonTooth, 7 });
        }

        return stock;
    }

    #endregion injected subroutines
}
