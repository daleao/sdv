namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
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

        if (!WeaponsModule.Config.InfinityPlusOne && !WeaponsModule.Config.DwarvishLegacy)
        {
            return true; // run original logic
        }

        if (!questionAndAnswer.ContainsAnyOf("DarkSword_", "Blacksmith_", "Yoba_"))
        {
            return true; // run original logic
        }

        try
        {
            var player = Game1.player;
            switch (questionAndAnswer)
            {
                case "DarkSword_GrabIt":
                {
                    Game1.activeClickableMenu.exitThisMenuNoSound();
                    Game1.playSound("parry");
                    player.addItemByMenuIfNecessaryElseHoldUp(new MeleeWeapon(ItemIDs.DarkSword));
                    player.mailReceived.Add("gotDarkSword");
                    break;
                }

                case "DarkSword_LeaveIt":
                {
                    break;
                }

                case "Blacksmith_Forge" when Globals.DwarvishBlueprintIndex.HasValue &&
                                             Globals.DwarvenScrapIndex.HasValue && Globals.ElderwoodIndex.HasValue:
                {
                    Game1.activeClickableMenu = new ShopMenu(
                        GetBlacksmithForgeStock(), 0, "ClintForge");
                    break;
                }

                default:
                {
                    var split = questionAndAnswer.SplitWithoutAllocation('_');
                    if (!split[0].Equals("Yoba", StringComparison.Ordinal))
                    {
                        return true; // run original logic
                    }

                    switch (split[1].ToString())
                    {
                        case "Honor":
                            Game1.drawObjectDialogue(Virtue.Honor.FlavorText);
                            player.Write(DataKeys.HasReadHonor, true.ToString());
                            break;
                        case "Compassion":
                            Game1.drawObjectDialogue(Virtue.Compassion.FlavorText);
                            player.Write(DataKeys.HasReadCompassion, true.ToString());
                            break;
                        case "Wisdom":
                            Game1.drawObjectDialogue(Virtue.Wisdom.FlavorText);
                            player.Write(DataKeys.HasReadWisdom, true.ToString());
                            break;
                        case "Generosity":
                            Game1.drawObjectDialogue(Virtue.Generosity.FlavorText);
                            player.Write(DataKeys.HasReadGenerosity, true.ToString());
                            break;
                        case "Valor":
                            Game1.drawObjectDialogue(Virtue.Valor.FlavorText);
                            player.Write(DataKeys.HasReadValor, true.ToString());
                            break;
                    }

                    if (!player.Read<bool>(DataKeys.HasReadHonor) ||
                        !player.Read<bool>(DataKeys.HasReadCompassion) ||
                        !player.Read<bool>(DataKeys.HasReadWisdom) ||
                        !player.Read<bool>(DataKeys.HasReadGenerosity) ||
                        !player.Read<bool>(DataKeys.HasReadValor))
                    {
                        return false; // don't run original logic
                    }

                    player.addQuest(Virtue.Honor);
                    player.addQuest(Virtue.Compassion);
                    player.addQuest(Virtue.Wisdom);
                    player.addQuest(Virtue.Generosity);
                    player.addQuest(Virtue.Valor);
                    player.completeQuest((int)Quest.CurseIntro);
                    Virtue.List.ForEach(virtue => virtue.CheckForCompletion(Game1.player));
                    return false; // don't run original logic
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
        var found = Game1.player.Read(DataKeys.BlueprintsFound).ParseList<int>().ToHashSet();

        if (found.Contains(ItemIDs.ForestSword))
        {
            stock.Add(
                new MeleeWeapon(ItemIDs.ForestSword),
                new[] { 0, int.MaxValue, Globals.ElderwoodIndex!.Value, 2 });
        }

        if (found.Contains(ItemIDs.ElfBlade))
        {
            stock.Add(
                new MeleeWeapon(ItemIDs.ElfBlade),
                new[] { 0, int.MaxValue, Globals.ElderwoodIndex!.Value, 1 });
        }

        if (found.Contains(ItemIDs.DwarfSword))
        {
            stock.Add(
                new MeleeWeapon(ItemIDs.DwarfSword),
                new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex!.Value, 5 });
        }

        if (found.Contains(ItemIDs.DwarfHammer))
        {
            stock.Add(
                new MeleeWeapon(ItemIDs.DwarfHammer),
                new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex!.Value, 5 });
        }

        if (found.Contains(ItemIDs.DwarfDagger))
        {
            stock.Add(
                new MeleeWeapon(ItemIDs.DwarfDagger),
                new[] { 0, int.MaxValue, Globals.DwarvenScrapIndex!.Value, 3 });
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
