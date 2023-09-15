namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Dwarven;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Constants;
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

        if (questionAndAnswer != "Blacksmith_Forge" || !JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue)
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

        if (JsonAssetsIntegration.ElderwoodIndex.HasValue)
        {
            if (found.Contains(WeaponIds.ForestSword))
            {
                stock.Add(
                    new MeleeWeapon(WeaponIds.ForestSword),
                    new[] { 0, int.MaxValue, JsonAssetsIntegration.ElderwoodIndex.Value, 2 });
            }

            if (found.Contains(WeaponIds.ElfBlade))
            {
                stock.Add(
                    new MeleeWeapon(WeaponIds.ElfBlade),
                    new[] { 0, int.MaxValue, JsonAssetsIntegration.ElderwoodIndex.Value, 1 });
            }
        }

        if (JsonAssetsIntegration.DwarvenScrapIndex.HasValue)
        {
            if (found.Contains(WeaponIds.DwarfSword))
            {
                stock.Add(
                    new MeleeWeapon(WeaponIds.DwarfSword),
                    new[] { 0, int.MaxValue, JsonAssetsIntegration.DwarvenScrapIndex.Value, 5 });
            }

            if (found.Contains(WeaponIds.DwarfHammer))
            {
                stock.Add(
                    new MeleeWeapon(WeaponIds.DwarfHammer),
                    new[] { 0, int.MaxValue, JsonAssetsIntegration.DwarvenScrapIndex.Value, 5 });
            }

            if (found.Contains(WeaponIds.DwarfDagger))
            {
                stock.Add(
                    new MeleeWeapon(WeaponIds.DwarfDagger),
                    new[] { 0, int.MaxValue, JsonAssetsIntegration.DwarvenScrapIndex.Value, 3 });
            }
        }

        if (found.Contains(WeaponIds.DragontoothCutlass))
        {
            stock.Add(
                new MeleeWeapon(WeaponIds.DragontoothCutlass),
                new[] { 0, int.MaxValue, ObjectIds.DragonTooth, 10 });
        }

        if (found.Contains(WeaponIds.DragontoothClub))
        {
            stock.Add(
                new MeleeWeapon(WeaponIds.DragontoothClub),
                new[] { 0, int.MaxValue, ObjectIds.DragonTooth, 10 });
        }

        if (found.Contains(WeaponIds.DragontoothShiv))
        {
            stock.Add(
                new MeleeWeapon(WeaponIds.DragontoothShiv),
                new[] { 0, int.MaxValue, ObjectIds.DragonTooth, 7 });
        }

        return stock;
    }

    #endregion injected subroutines
}
