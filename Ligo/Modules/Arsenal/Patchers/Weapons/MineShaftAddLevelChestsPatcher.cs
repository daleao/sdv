namespace DaLion.Ligo.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftAddLevelChestsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftAddLevelChestsPatcher"/> class.</summary>
    internal MineShaftAddLevelChestsPatcher()
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.getSpecialItemForThisMineLevel));
    }

    #region harmony patches

    /// <summary>Cheapen mine chests to avoid trivializing gear.</summary>
    [HarmonyPrefix]
    private static bool MineShaftAddLevelChestsPrefix(MineShaft __instance)
    {
        if (!ModEntry.Config.Arsenal.Weapons.RebalancedWeapons)
        {
            return true; // run original logic
        }

        var chestItems = new List<Item>();
        var chestSpot = new Vector2(9f, 9f);
        var tint = Color.White;
        if (__instance.mineLevel < 121 && __instance.mineLevel % 20 == 0 && __instance.mineLevel % 40 != 0)
        {
            chestSpot.Y += 4f;
        }

        var forceTreasureRoom = false;
        switch (__instance.mineLevel)
        {
            case 5:
                Game1.player.completeQuest(14);
                if (!Game1.player.hasOrWillReceiveMail("guildQuest"))
                {
                    Game1.addMailForTomorrow("guildQuest");
                }

                break;
            case 20:
                chestItems.Add(new Slingshot());
                break;
            case 40:
                Game1.player.completeQuest(17);
                chestItems.Add(MineShaft.getTreasureRoomItem());
                break;
            case 60:
                chestItems.Add(new Slingshot(Constants.MasterSlingshotIndex));
                break;
            case 80:
                chestItems.Add(MineShaft.getTreasureRoomItem());
                break;
            case 100:
                chestItems.Add(new SObject(434, 1));
                break;
            case 120:
                Game1.player.completeQuest(18);
                Game1.getSteamAchievement("Achievement_TheBottom");
                if (!Game1.player.hasSkullKey)
                {
                    chestItems.Add(new SpecialItem(4));
                }

                tint = Color.Pink;
                break;
            case 220:
                if (Game1.player.secretNotesSeen.Contains(10) && !Game1.player.mailReceived.Contains("qiCave"))
                {
                    Game1.eventUp = true;
                    Game1.displayHUD = false;
                    Game1.player.CanMove = false;
                    Game1.player.showNotCarrying();
                    __instance.currentEvent = new Event(Game1.content.LoadString(MineShaft.numberOfCraftedStairsUsedThisRun <= 10
                        ? "Data\\ExtraDialogue:SkullCavern_100_event_honorable"
                        : "Data\\ExtraDialogue:SkullCavern_100_event"));
                    __instance.currentEvent.exitLocation = new LocationRequest(__instance.Name, isStructure: false, __instance);
                    Game1.player.chestConsumedMineLevels[__instance.mineLevel] = true;
                }
                else
                {
                    forceTreasureRoom = true;
                }

                break;
            default:
                if (__instance.mineLevel % 10 == 0 && __instance.mineLevel < 120)
                {
                    chestItems.Add(GetTreasureItem(__instance.mineLevel));
                }

                break;
        }

        if (ModEntry.Reflector.GetUnboundFieldGetter<MineShaft, NetBool>(__instance, "netIsTreasureRoom").Invoke(__instance).Value || forceTreasureRoom)
        {
            chestItems.Add(MineShaft.getTreasureRoomItem());
        }

        if (chestItems.Count > 0 && !Game1.player.chestConsumedMineLevels.ContainsKey(__instance.mineLevel))
        {
            __instance.overlayObjects[chestSpot] = new Chest(0, chestItems, chestSpot)
            {
                Tint = tint,
            };
        }

        return false; // don't run original logic
    }

    #endregion harmony patches

    #region injected subroutines

    private static Item GetTreasureItem(int level)
    {
        return Game1.random.Next(14) switch
        {
            1 => new SObject(286, 15), // cherry bomb
            2 => new SObject(287, 10), // bomb
            3 => new SObject(288, 5), // mega bomb
            4 => new SObject(773, Game1.random.Next(2, 5)), // life elixir
            5 => new SObject(349, Game1.random.Next(2, 5)), // energy tonic
            6 => new SObject(749, 5), // omni geode
            7 => new SObject(688, 5), // warp totem (farm)
            8 => new SObject(681, Game1.random.Next(1, 4)), // rain totem
            9 => new SObject(Game1.random.Next(472, 499), Game1.random.Next(1, 5) * 5), // seeds
            10 => new SObject(Game1.random.Next(628, 634), 1), // tree sapling
            11 => new SObject(621, 2), // quality sprinkler
            12 => new SObject(level < 40 ? 680 : level < 80 ? 413 : 437, 1), // slime egg
            13 => new SObject(Game1.random.Next(235, 245), 5), // food
            _ => new SObject(level < 40 ? SObject.copperBar : level < 80 ? SObject.ironBar : SObject.goldBar, Game1.random.Next(2, 4)),
        };
    }

    #endregion injected subroutines
}
