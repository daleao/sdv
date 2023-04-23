namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DaLion.Shared.Extensions;
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
        this.Target = this.RequireMethod<MineShaft>("addLevelChests");
    }

    #region harmony patches

    /// <summary>Cheapen mine chests to avoid trivializing gear.</summary>
    [HarmonyPrefix]
    private static bool MineShaftAddLevelChestsPrefix(MineShaft __instance)
    {
        if (!WeaponsModule.Config.EnableRebalance)
        {
            return true; // run original logic
        }

        try
        {
            var player = Game1.player;
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
                    player.completeQuest(14);
                    if (!player.hasOrWillReceiveMail("guildQuest"))
                    {
                        Game1.addMailForTomorrow("guildQuest");
                    }

                    break;
                case 20:
                    chestItems.Add(new Slingshot());
                    break;
                case 40:
                    player.completeQuest(17);
                    goto default;
                case 60:
                    chestItems.Add(new Slingshot(ItemIDs.MasterSlingshot));
                    break;
                case 100:
                    chestItems.Add(new SObject(434, 1)); // stardrop
                    break;
                case 120:
                    player.completeQuest(18);
                    Game1.getSteamAchievement("Achievement_TheBottom");
                    if (!player.hasSkullKey)
                    {
                        chestItems.Add(new SpecialItem(4));
                    }

                    tint = Color.Pink;
                    break;
                case 220:
                    if (player.secretNotesSeen.Contains(10) && !player.mailReceived.Contains("qiCave"))
                    {
                        Game1.eventUp = true;
                        Game1.displayHUD = false;
                        player.CanMove = false;
                        player.showNotCarrying();
                        __instance.currentEvent = new Event(Game1.content.LoadString(
                            MineShaft.numberOfCraftedStairsUsedThisRun <= 10
                                ? "Data\\ExtraDialogue:SkullCavern_100_event_honorable"
                                : "Data\\ExtraDialogue:SkullCavern_100_event"));
                        __instance.currentEvent.exitLocation =
                            new LocationRequest(__instance.Name, isStructure: false, __instance);
                        player.chestConsumedMineLevels[__instance.mineLevel] = true;
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

            if (Reflector.GetUnboundFieldGetter<MineShaft, NetBool>(__instance, "netIsTreasureRoom").Invoke(__instance)
                    .Value || forceTreasureRoom)
            {
                chestItems.Add(MineShaft.getTreasureRoomItem());
            }

            if (chestItems.Count > 0 && !Game1.player.chestConsumedMineLevels.ContainsKey(__instance.mineLevel))
            {
                __instance.overlayObjects[chestSpot] = new Chest(0, chestItems, chestSpot) { Tint = tint, };
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

    #region injected subroutines

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for inner functions.")]
    private static Item GetTreasureItem(int level)
    {
        // force a new RNG to allow save-scumming
        var r = new Random(Guid.NewGuid().GetHashCode());
        return r.Next(8) switch
        {
            1 => chooseBomb(r),
            2 => chooseTotem(r),
            3 => new SObject(r.Next(235, 245), r.Next(3, 8)), // food
            4 => new SObject(r.Choose(773, 349), r.Next(4, 10)), // life elixir or energy tonic
            5 => new SObject(r.Choose(369, 371, 466), 20), // level-2 fertilizer
            6 => new SObject(621, 2), // quality sprinkler
            _ => new SObject(level < 40 ? SObject.copperBar : level < 80 ? SObject.ironBar : SObject.goldBar, r.Next(5, 15)),
        };

        SObject chooseBomb(Random rr)
        {
            var which = rr.Next(286, 289);
            return new SObject(which, 20 - ((which * 5) - 1430));
        }

        SObject chooseTotem(Random rr)
        {
            var which = rr.Choose(688, 681);
            return new SObject(which, which == 681 ? rr.Next(2, 5) : rr.Next(3, 8));
        }
    }

    #endregion injected subroutines
}
